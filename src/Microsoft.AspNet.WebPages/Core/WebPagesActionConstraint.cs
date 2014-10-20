// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

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

        public int Order { get { return DefaultOrder.DefaultFrameworkSortOrder; } }

        public bool Accept([NotNull] ActionConstraintContext context)
        {
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
