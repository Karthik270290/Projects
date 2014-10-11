using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesActionDescriptorProvider : INestedProvider<ActionDescriptorProviderContext>
    {
        private readonly IControllerAssemblyProvider _controllerAssemblyProvider;
        private readonly IActionDiscoveryConventions _conventions;
        private readonly IReadOnlyList<IFilter> _globalFilters;
        private readonly IEnumerable<IGlobalModelConvention> _modelConventions;
        private readonly string _webPagesUrlPrefix;
        private readonly string _webPagesFolderName;
        private readonly string _routedPagesFolderName;

        public WebPagesActionDescriptorProvider(IControllerAssemblyProvider controllerAssemblyProvider,
                                                 IActionDiscoveryConventions conventions,
                                                 IGlobalFilterProvider globalFilters,
                                                 IOptionsAccessor<MvcOptions> mvcOptionsAccessor,
                                                 IOptionsAccessor<WebPagesOptions> webPagesOptionsAccessor)
        {
            _controllerAssemblyProvider = controllerAssemblyProvider;
            _conventions = conventions;
            _globalFilters = globalFilters.Filters;
            _modelConventions = mvcOptionsAccessor.Options.ApplicationModelConventions;
            _webPagesUrlPrefix = webPagesOptionsAccessor.Options.PagesUrlPrefix;
            _webPagesFolderName = webPagesOptionsAccessor.Options.PagesFolderPath;
            _routedPagesFolderName = webPagesOptionsAccessor.Options.RoutedPagesFolderPath;
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

            var action = applicationModel.Controllers.Single().Actions.Single();
            action.AttributeRouteModel = new AttributeRouteModel(
                new WebPagesCoordinator.RouteTemplate(_webPagesUrlPrefix));

            return applicationModel;
        }
    }
}