// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesActionConstraint : IActionConstraint
    {
        public static readonly string WebPagesDefaultRouteKey = "__route";
        
        private readonly ICompositeViewEngine _viewEngine;
        private readonly string _constraintPath;

        public WebPagesActionConstraint(ICompositeViewEngine viewEngine, [NotNull] string constraintPath)
        {
            _viewEngine = viewEngine;
            _constraintPath = constraintPath;
        }

        public int Order { get { return -10000; } } // TODO: This is probably not necessary

        public bool Accept([NotNull] ActionConstraintContext context)
        {
            var defaults = context.CurrentCandidate.Action.RouteValueDefaults;

            // At this stage, the default route values from an action are not added yet
            // to the route context, since the action wasn't yet selected.
            object defaultValue;

            if (defaults.TryGetValue(WebPagesDefaultRouteKey, out defaultValue) &&
                defaultValue != null)
            {
                return true;
            }

            var routeValues = context.RouteContext.RouteData.Values;

            var viewPath = _constraintPath
                + "/"
                + (string)routeValues[Coordinator.ViewPathRouteKey];

            ActionContext actionContext = new ActionContext(context.RouteContext,
                                                      context.CurrentCandidate.Action);

            var result = _viewEngine.FindView(actionContext, viewPath);

            if (result.Success)
            {
                // need to measure if we should cache the view
                return true;
            }

            return false;
        }
    }
}
