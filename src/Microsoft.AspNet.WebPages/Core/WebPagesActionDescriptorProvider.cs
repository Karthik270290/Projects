using System;
using System.Collections.Generic;
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
            _routedPagesFolderName = webPagesOptionsAccessor.Options.RoutedPagesFolderName;
        }

        public int Order
        {
            get { return DefaultOrder.DefaultFrameworkSortOrder - 100; }
        }

        public void Invoke(ActionDescriptorProviderContext context, Action callNext)
        {
            callNext();
        }
    }
}