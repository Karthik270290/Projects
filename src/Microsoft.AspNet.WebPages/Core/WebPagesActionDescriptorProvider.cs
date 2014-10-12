// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesActionDescriptorProvider : INestedProvider<ActionDescriptorProviderContext>
    {
        private readonly IActionDiscoveryConventions _conventions;
        private readonly IReadOnlyList<IFilter> _globalFilters;
        private readonly IEnumerable<IGlobalModelConvention> _modelConventions;
        private readonly ICompilerCache _compilerCache;
        private readonly string _webPagesUrlPrefix;
        private readonly string _webPagesFolderName;
        private readonly string _routedPagesFolderName;

        public WebPagesActionDescriptorProvider(IActionDiscoveryConventions conventions,
                                                IGlobalFilterProvider globalFilters,
                                                IOptionsAccessor<MvcOptions> mvcOptionsAccessor,
                                                IOptionsAccessor<WebPagesOptions> webPagesOptionsAccessor,
                                                ICompilerCache compilerCache)
        {
            _conventions = conventions;
            _globalFilters = globalFilters.Filters;
            _modelConventions = mvcOptionsAccessor.Options.ApplicationModelConventions;
            _webPagesUrlPrefix = webPagesOptionsAccessor.Options.PagesUrlPrefix;
            _webPagesFolderName = webPagesOptionsAccessor.Options.PagesFolderPath;
            _compilerCache = compilerCache;

            var path = webPagesOptionsAccessor.Options.RoutedPagesFolderPath;

            path = path.Replace('/', '\\').TrimStart(new[] { '\\' }) + "\\";
            _routedPagesFolderName = path;
        }

        public int Order
        {
            get { return DefaultOrder.DefaultFrameworkSortOrder - 100; }
        }

        public void Invoke(ActionDescriptorProviderContext context, Action callNext)
        {
            context.Results.AddRange(GetDescriptors());

            callNext();
        }

        public IEnumerable<ControllerActionDescriptor> GetDescriptors()
        {
            var model = BuildModel();

            // apply conventions but no global conventions apply here.
            model.ApplyConventions(Enumerable.Empty<IGlobalModelConvention>());

            return ActionDescriptorBuilder.Build(model);
        }

        public GlobalModel BuildModel()
        {
            var applicationModel = new GlobalModel();
            applicationModel.Filters.AddRange(_globalFilters);

            applicationModel.AddControllerModel(typeof(PageCoordinator).GetTypeInfo(), _conventions);

            var catchAllcontroller = applicationModel.Controllers.Single();
            var action = catchAllcontroller.Actions.Single();

            // Add the default route to match disk paths for views.
            action.AttributeRouteModel = new AttributeRouteModel(
                new CatchAllRouteTemplate(_webPagesUrlPrefix));

            if (!string.IsNullOrEmpty(
                System.Environment.GetEnvironmentVariable("DebugBreak")))
            {
                System.Diagnostics.Debugger.Launch();
                System.Diagnostics.Debugger.Break();
            }

            ControllerModel routedController = null;
            ActionModel baseAction = null;

            // Add routed views.
            foreach (var value in _compilerCache.Values)
            {
                if (!string.IsNullOrEmpty(value.Value.Route))
                {
                    if (baseAction == null)
                    {
                        var typeInfo = typeof(RoutedPageCoordinator).GetTypeInfo();
                        applicationModel.AddControllerModel(typeInfo, _conventions);

                        routedController = applicationModel
                                                    .Controllers
                                                    .Single(c => c.ControllerType == typeInfo);

                        baseAction = routedController.Actions.Single();
                        routedController.Actions.Clear();
                    }

                    AddRoutedAction(routedController, baseAction, value);
                }
            }

            return applicationModel;
        }

        private void AddRoutedAction(ControllerModel controller,
                                     ActionModel baseAction,
                                     KeyValuePair<string, CompilerCacheEntry> entry)
        {
            var relativePath = entry.Key ?? string.Empty;
            if (!string.IsNullOrEmpty(_routedPagesFolderName) &&
                !relativePath.StartsWith(_routedPagesFolderName))
            {
                throw new InvalidOperationException("Route views have to be under the routed pages path.");
            }

            var routedAction = new ActionModel(baseAction);
            routedAction.AttributeRouteModel = new AttributeRouteModel(
                new RouteTemplate(entry.Value.Route));

            routedAction.AdditionalDefaults = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            string viewPath = "/" + entry.Key.Replace('\\', '/').TrimStart('/');
            routedAction.AdditionalDefaults.Add(Coordinator.ViewPathRouteKey, viewPath);
            routedAction.AdditionalDefaults.Add(WebPagesActionConstraint.WebPagesDefaultRouteKey,
                                                entry.Value.Route);

            controller.Actions.Add(routedAction);
        }
    }
}
