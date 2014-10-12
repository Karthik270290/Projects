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
            _routedPagesFolderName = webPagesOptionsAccessor.Options.RoutedPagesFolderPath;
            _compilerCache = compilerCache;
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

            applicationModel.AddControllerModel(typeof(WebPagesCoordinator).GetTypeInfo(), _conventions);

            var controller = applicationModel.Controllers.Single();
            var action = controller.Actions.Single();

            // Add the default route to match disk paths for views.
            action.AttributeRouteModel = new AttributeRouteModel(
                new WebPagesCoordinator.CatchAllRouteTemplate(_webPagesUrlPrefix));

            System.Diagnostics.Debugger.Launch();
            System.Diagnostics.Debugger.Break();
            
            // Add routed views.
            foreach (var value in _compilerCache.Values)
            {
                if (!string.IsNullOrEmpty(value.Value.Route))
                {
                    AddRoutedAction(controller, action, value);
                }
            }

            return applicationModel;
        }

        private void AddRoutedAction(ControllerModel controllerModel,
                                     ActionModel originalAction,
                                     KeyValuePair<string, CompilerCacheEntry> entry)
        {
            var relativePath = entry.Key ?? string.Empty;
            if (!string.IsNullOrEmpty(_routedPagesFolderName) &&
                !relativePath.StartsWith(_routedPagesFolderName)) // TODO handle / and \ at end of _routedPages                
            {
                throw new InvalidOperationException("Route views have to be under the routed path");
            }

            var newAction = new ActionModel(originalAction);
            newAction.AttributeRouteModel = new AttributeRouteModel(
                new WebPagesCoordinator.CatchAllRouteTemplate(entry.Value.Route));

            newAction.AdditionalDefaults = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            newAction.AdditionalDefaults.Add("__viewPath", entry.Key);
            newAction.AdditionalDefaults.Add("__route", entry.Value.Route);

            controllerModel.Actions.Add(newAction);
        }
    }
}
