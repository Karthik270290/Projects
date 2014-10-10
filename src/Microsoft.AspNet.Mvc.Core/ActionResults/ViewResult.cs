// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class ViewResult : ActionResult
    {
        public string ViewName { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public override async Task ExecuteResultAsync([NotNull] ActionContext context)
        {
            var viewEngine = ViewEngine ?? context.HttpContext.RequestServices.GetService<ICompositeViewEngine>();

            var viewName = ViewName ?? context.ActionDescriptor.Name;
            var view = FindView(viewEngine, context, viewName);

            await ViewExecuter.ExecuteView(view, context, ViewData);
        }

        protected static IView FindView(IViewEngine viewEngine, ActionContext context, string viewName)
        {
            var result = viewEngine.FindView(context, viewName);
            if (!result.Success)
            {
                var locations = string.Empty;
                if (result.SearchedLocations != null)
                {
                    locations = Environment.NewLine +
                        string.Join(Environment.NewLine, result.SearchedLocations);
                }

                throw new InvalidOperationException(Resources.FormatViewEngine_ViewNotFound(viewName, locations));
            }

            return result.View;
        }
    }
}

