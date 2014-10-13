// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.FileSystems;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesActionDescriptorProvider : INestedProvider<ActionDescriptorProviderContext>
    {
        private readonly IActionDiscoveryConventions _conventions;
        private readonly IReadOnlyList<IFilter> _globalFilters;
        private readonly IEnumerable<IGlobalModelConvention> _modelConventions;
        private readonly ICompilerCache _compilerCache;
        private readonly IApplicationEnvironment _appEnv;
        private readonly string _webPagesUrlPrefix;
        private readonly string _webPagesFolderName;
        private readonly string _routedPagesFolderName;
        private readonly bool _updatePrecompilation;

        public WebPagesActionDescriptorProvider(IActionDiscoveryConventions conventions,
                                                IGlobalFilterProvider globalFilters,
                                                IOptionsAccessor<MvcOptions> mvcOptionsAccessor,
                                                IOptionsAccessor<WebPagesOptions> webPagesOptionsAccessor,
                                                IApplicationEnvironment appEnv,
                                                ICompilerCache compilerCache)
        {
            _conventions = conventions;
            _globalFilters = globalFilters.Filters;
            _modelConventions = mvcOptionsAccessor.Options.ApplicationModelConventions;
            _webPagesUrlPrefix = webPagesOptionsAccessor.Options.PagesUrlPrefix;
            _webPagesFolderName = webPagesOptionsAccessor.Options.PagesFolderPath;
            _updatePrecompilation = webPagesOptionsAccessor.Options.UpdateRoutesFromPrecompilationAtStartup;
            _appEnv = appEnv;
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

            if (_updatePrecompilation) // TODO: move to a background thread at startup
            {
                ScanForRoutedPages(applicationModel);
            }
            else
            {
                GetValuesFromPrecompilation(applicationModel);
            }

            return applicationModel;
        }

        private void ScanForRoutedPages(GlobalModel applicationModel)
        {
            BaseRoutedModel model = null;
            var directory = new RazorDirectory(new PhysicalFileSystem(_appEnv.ApplicationBasePath),
                                               ".cshtml");

            foreach (var relativeFileInfo in directory.GetFileInfos(_routedPagesFolderName))
            {
                var route = RazorRoute.GetRoute(relativeFileInfo.FileInfo);

                if (route != null)
                {
                    model = model ?? CreateBaseRoutedModel(applicationModel);

                    AddRoutedAction(model, route, relativeFileInfo.RelativePath);
                }
            }
        }

        private void GetValuesFromPrecompilation(GlobalModel applicationModel)
        {
            BaseRoutedModel model = null;

            foreach (var value in _compilerCache.Values)
            {
                if (!string.IsNullOrEmpty(value.Value.Route))
                {
                    model = model ?? CreateBaseRoutedModel(applicationModel);

                    AddRoutedAction(model, value);
                }
            }
        }

        private BaseRoutedModel CreateBaseRoutedModel(GlobalModel applicationModel)
        {
            var typeInfo = typeof(RoutedPageCoordinator).GetTypeInfo();
            applicationModel.AddControllerModel(typeInfo, _conventions);

            var routedController = applicationModel
                                        .Controllers
                                        .Single(c => c.ControllerType == typeInfo);

            var baseAction = routedController.Actions.Single();
            routedController.Actions.Clear();

            return new BaseRoutedModel()
            {
                Controller = routedController,
                Action = baseAction,
            };
        }

        private void AddRoutedAction(BaseRoutedModel model,
                                     KeyValuePair<string, CompilerCacheEntry> entry)
        {
            AddRoutedAction(model, entry.Value.Route, entry.Key ?? string.Empty);
        }

        private void AddRoutedAction(BaseRoutedModel model,
                                    string route,
                                    string relativePath)
        {
            if (!string.IsNullOrEmpty(_routedPagesFolderName) &&
                !relativePath.StartsWith(_routedPagesFolderName))
            {
                throw new InvalidOperationException("Route views have to be under the routed pages path.");
            }

            var routedAction = new ActionModel(model.Action);
            routedAction.AttributeRouteModel = new AttributeRouteModel(
                new RouteTemplate(route));

            routedAction.AdditionalDefaults = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            string viewPath = "/" + relativePath.Replace('\\', '/').TrimStart('/');
            routedAction.AdditionalDefaults.Add(Coordinator.ViewPathRouteKey, viewPath);
            routedAction.AdditionalDefaults.Add(WebPagesActionConstraint.WebPagesDefaultRouteKey,
                                                route);

            model.Controller.Actions.Add(routedAction);
        }

        private class BaseRoutedModel
        {
            public ControllerModel Controller { get; set; }
            public ActionModel Action { get; set; }
        }
    }
}
