// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Mvc
{
    public class ControllerActionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly IAssemblyProvider _assemblyProvider;
        private readonly IActionDiscoveryConventions _conventions;
        private readonly IReadOnlyList<IFilter> _globalFilters;
        private readonly IEnumerable<IGlobalModelConvention> _modelConventions;

        public ControllerActionDescriptorProvider(IAssemblyProvider assemblyProvider,
                                                 IActionDiscoveryConventions conventions,
                                                 IGlobalFilterProvider globalFilters,
                                                 IOptions<MvcOptions> optionsAccessor)
        {
            _assemblyProvider = assemblyProvider;
            _conventions = conventions;
            _globalFilters = globalFilters.Filters;
            _modelConventions = optionsAccessor.Options.ApplicationModelConventions;
        }

        public int Order
        {
            get { return DefaultOrder.DefaultFrameworkSortOrder; }
        }

        public void Invoke(ActionDescriptorProviderContext context, Action callNext)
        {
            context.Results.AddRange(GetDescriptors());
            callNext();
        }

        public IEnumerable<ControllerActionDescriptor> GetDescriptors()
        {
            var model = BuildModel();
            model.ApplyConventions(_modelConventions);
            return ActionDescriptorBuilder.Build(model);
        }

        public GlobalModel BuildModel()
        {
            var applicationModel = new GlobalModel();
            applicationModel.Filters.AddRange(_globalFilters);

            var assemblies = _assemblyProvider.CandidateAssemblies;
            var types = assemblies.SelectMany(a => a.DefinedTypes);
            var controllerTypes = types.Where(_conventions.IsController);

            foreach (var controllerType in controllerTypes)
            {
                applicationModel.AddControllerModel(controllerType, _conventions);
            }

            return applicationModel;
        }
    }
}
