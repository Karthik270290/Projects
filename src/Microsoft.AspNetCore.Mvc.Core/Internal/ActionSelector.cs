// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.Internal
{
    /// <summary>
    /// A default <see cref="IActionSelector"/> implementation.
    /// </summary>
    public class ActionSelector : IActionSelector
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ActionConstraintCache _actionConstraintCache;
        private readonly ILogger _logger;

        private Cache _cache;

        /// <summary>
        /// Creates a new <see cref="ActionSelector"/>.
        /// </summary>
        /// <param name="actionDescriptorCollectionProvider">
        /// The <see cref="IActionDescriptorCollectionProvider"/>.
        /// </param>
        /// <param name="actionConstraintCache">The <see cref="ActionConstraintCache"/> that
        /// providers a set of <see cref="IActionConstraint"/> instances.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public ActionSelector(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            ActionConstraintCache actionConstraintCache,
            ILoggerFactory loggerFactory)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _logger = loggerFactory.CreateLogger<ActionSelector>();
            _actionConstraintCache = actionConstraintCache;
        }

        private Cache Current
        {
            get
            {
                var actions = _actionDescriptorCollectionProvider.ActionDescriptors;
                var cache = Volatile.Read(ref _cache);

                if (cache != null && cache.Version == actions.Version)
                {
                    return cache;
                }

                cache = new Cache(actions);
                _cache = cache;
                return cache;
            }
        }

        /// <inheritdoc />
        public ActionDescriptor Select(RouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var cache = Current;

            var values = new string[cache.RouteKeys.Length];
            for (var i = 0; i < cache.RouteKeys.Length; i++)
            {
                object value;
                context.RouteData.Values.TryGetValue(cache.RouteKeys[i], out value);

                if (value != null)
                {
                    values[i] = value as string ?? Convert.ToString(value);
                }
            }

            var ordinalKey = new ActionSelectionOrdinalKey(values);
            List<ActionDescriptor> matchingRouteValues;
            if (!cache.OrdinalEntries.TryGetValue(ordinalKey, out matchingRouteValues))
            {
                var ordinalIgnoreCaseKey = new ActionSelectionOrdinalIgnoreCaseKey(values);
                if (!cache.OrdinalIgnoreCaseEntries.TryGetValue(ordinalIgnoreCaseKey, out matchingRouteValues))
                {

                    _logger.NoActionsMatched();
                    return null;
                }
            }

            var candidates = new List<ActionSelectorCandidate>();

            // Perf: Avoid allocations
            for (var i = 0; i < matchingRouteValues.Count; i++)
            {
                var action = matchingRouteValues[i];
                var constraints = _actionConstraintCache.GetActionConstraints(context.HttpContext, action);
                candidates.Add(new ActionSelectorCandidate(action, constraints));
            }

            var matchingActionConstraints =
                EvaluateActionConstraints(context, candidates, startingOrder: null);

            List<ActionDescriptor> matchingActions = null;
            if (matchingActionConstraints != null)
            {
                matchingActions = new List<ActionDescriptor>(matchingActionConstraints.Count);
                // Perf: Avoid allocations
                for (var i = 0; i < matchingActionConstraints.Count; i++)
                {
                    var candidate = matchingActionConstraints[i];
                    matchingActions.Add(candidate.Action);
                }
            }

            var finalMatches = SelectBestActions(matchingActions);

            if (finalMatches == null || finalMatches.Count == 0)
            {
                return null;
            }
            else if (finalMatches.Count == 1)
            {
                var selectedAction = finalMatches[0];

                return selectedAction;
            }
            else
            {
                var actionNames = string.Join(
                    Environment.NewLine,
                    finalMatches.Select(a => a.DisplayName));

                _logger.AmbiguousActions(actionNames);

                var message = Resources.FormatDefaultActionSelector_AmbiguousActions(
                    Environment.NewLine,
                    actionNames);

                throw new AmbiguousActionException(message);
            }
        }

        /// <summary>
        /// Returns the set of best matching actions.
        /// </summary>
        /// <param name="actions">The set of actions that satisfy all constraints.</param>
        /// <returns>A list of the best matching actions.</returns>
        protected virtual IReadOnlyList<ActionDescriptor> SelectBestActions(IReadOnlyList<ActionDescriptor> actions)
        {
            return actions;
        }

        private IReadOnlyList<ActionSelectorCandidate> EvaluateActionConstraints(
            RouteContext context,
            IReadOnlyList<ActionSelectorCandidate> candidates,
            int? startingOrder)
        {
            // Find the next group of constraints to process. This will be the lowest value of
            // order that is higher than startingOrder.
            int? order = null;

            // Perf: Avoid allocations
            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                if (candidate.Constraints != null)
                {
                    for (var j = 0; j < candidate.Constraints.Count; j++)
                    {
                        var constraint = candidate.Constraints[j];
                        if ((startingOrder == null || constraint.Order > startingOrder) &&
                            (order == null || constraint.Order < order))
                        {
                            order = constraint.Order;
                        }
                    }
                }
            }

            // If we don't find a 'next' then there's nothing left to do.
            if (order == null)
            {
                return candidates;
            }

            // Since we have a constraint to process, bisect the set of actions into those with and without a
            // constraint for the 'current order'.
            var actionsWithConstraint = new List<ActionSelectorCandidate>();
            var actionsWithoutConstraint = new List<ActionSelectorCandidate>();

            var constraintContext = new ActionConstraintContext();
            constraintContext.Candidates = candidates;
            constraintContext.RouteContext = context;

            // Perf: Avoid allocations
            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];
                var isMatch = true;
                var foundMatchingConstraint = false;

                if (candidate.Constraints != null)
                {
                    constraintContext.CurrentCandidate = candidate;
                    for (var j = 0; j < candidate.Constraints.Count; j++)
                    {
                        var constraint = candidate.Constraints[j];
                        if (constraint.Order == order)
                        {
                            foundMatchingConstraint = true;

                            if (!constraint.Accept(constraintContext))
                            {
                                isMatch = false;
                                _logger.ConstraintMismatch(
                                    candidate.Action.DisplayName,
                                    candidate.Action.Id,
                                    constraint);
                                break;
                            }
                        }
                    }
                }

                if (isMatch && foundMatchingConstraint)
                {
                    actionsWithConstraint.Add(candidate);
                }
                else if (isMatch)
                {
                    actionsWithoutConstraint.Add(candidate);
                }
            }

            // If we have matches with constraints, those are 'better' so try to keep processing those
            if (actionsWithConstraint.Count > 0)
            {
                var matches = EvaluateActionConstraints(context, actionsWithConstraint, order);
                if (matches?.Count > 0)
                {
                    return matches;
                }
            }

            // If the set of matches with constraints can't work, then process the set without constraints.
            if (actionsWithoutConstraint.Count == 0)
            {
                return null;
            }
            else
            {
                return EvaluateActionConstraints(context, actionsWithoutConstraint, order);
            }
        }

        private class Cache
        {
            public Cache(ActionDescriptorCollection actions)
            {
                Version = actions.Version;

                OrdinalEntries = new Dictionary<ActionSelectionOrdinalKey, List<ActionDescriptor>>();
                OrdinalIgnoreCaseEntries = new Dictionary<ActionSelectionOrdinalIgnoreCaseKey, List<ActionDescriptor>>();

                var routeKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < actions.Items.Count; i++)
                {
                    var action = actions.Items[i];
                    foreach (var kvp in action.RouteValues)
                    {
                        routeKeys.Add(kvp.Key);
                    }
                }

                RouteKeys = routeKeys.ToArray();

                var actionsAndKeys = new ActionAndKeys[actions.Items.Count];

                for (var i = 0; i < actions.Items.Count; i++)
                {
                    var action = actions.Items[i];
                    var values = new string[RouteKeys.Length];
                    for (var j = 0; j < RouteKeys.Length; j++)
                    {
                        string value;
                        action.RouteValues.TryGetValue(RouteKeys[j], out value);

                        values[j] = value;
                    }

                    actionsAndKeys[i] = new ActionAndKeys(action, values);
                    
                    List<ActionDescriptor> entries;
                    if (!OrdinalIgnoreCaseEntries.TryGetValue(actionsAndKeys[i].OrdinalIgnoreCaseKey, out entries))
                    {
                        entries = new List<ActionDescriptor>();
                        OrdinalIgnoreCaseEntries.Add(actionsAndKeys[i].OrdinalIgnoreCaseKey, entries);
                    }

                    entries.Add(action);
                }

                for (var i = 0; i < actionsAndKeys.Length; i++)
                {
                    var actionAndKeys = actionsAndKeys[i];

                    var entries = OrdinalIgnoreCaseEntries[actionAndKeys.OrdinalIgnoreCaseKey];
                    OrdinalEntries[actionAndKeys.OrdinalKey] = entries;
                }
            }

            public int Version { get; set; }

            public string[] RouteKeys { get; set; }

            public Dictionary<ActionSelectionOrdinalKey, List<ActionDescriptor>> OrdinalEntries { get; set; }

            public Dictionary<ActionSelectionOrdinalIgnoreCaseKey, List<ActionDescriptor>> OrdinalIgnoreCaseEntries { get; set; }
        }

        private struct ActionAndKeys
        {
            public ActionAndKeys(ActionDescriptor actionDescriptor, string[] values)
            {
                ActionDescriptor = actionDescriptor;
                OrdinalKey = new ActionSelectionOrdinalKey(values);
                OrdinalIgnoreCaseKey = new ActionSelectionOrdinalIgnoreCaseKey(values);
            }

            public ActionDescriptor ActionDescriptor { get; }

            public ActionSelectionOrdinalKey OrdinalKey { get; }

            public ActionSelectionOrdinalIgnoreCaseKey OrdinalIgnoreCaseKey { get; }
        }

        private struct ActionSelectionOrdinalKey : IEquatable<ActionSelectionOrdinalKey>
        {
            private readonly string[] _values;

            public ActionSelectionOrdinalKey(string[] values)
            {
                _values = values;
            }

            public bool Equals(ActionSelectionOrdinalKey other)
            {
                if (_values.Length != other._values.Length)
                {
                    return false;
                }

                for (var i = 0; i < _values.Length; i++)
                {
                    var x = _values[i];
                    var y = other._values[i];

                    if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                    {
                        continue;
                    }

                    if (!string.Equals(x, y, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ActionSelectionOrdinalKey?;
                return other.HasValue ? Equals(other.Value) : false;
            }

            public override int GetHashCode()
            {
                var hash = new HashCodeCombiner();
                for (var i = 0; i < _values.Length; i++)
                {
                    hash.Add(_values[i], StringComparer.Ordinal);
                }

                return hash.CombinedHash;
            }
        }

        private struct ActionSelectionOrdinalIgnoreCaseKey : IEquatable<ActionSelectionOrdinalIgnoreCaseKey>
        {
            private readonly string[] _values;

            public ActionSelectionOrdinalIgnoreCaseKey(string[] values)
            {
                _values = values;
            }

            public bool Equals(ActionSelectionOrdinalIgnoreCaseKey other)
            {
                if (_values.Length != other._values.Length)
                {
                    return false;
                }

                for (var i = 0; i < _values.Length; i++)
                {
                    var x = _values[i];
                    var y = other._values[i];

                    if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                    {
                        continue;
                    }

                    if (!string.Equals(x, y, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ActionSelectionOrdinalIgnoreCaseKey?;
                return other.HasValue ? Equals(other.Value) : false;
            }
            
            public override int GetHashCode()
            {
                var hash = new HashCodeCombiner();
                for (var i = 0; i < _values.Length; i++)
                {
                    hash.Add(_values[i], StringComparer.OrdinalIgnoreCase);
                }

                return hash.CombinedHash;
            }
        }
    }
}
