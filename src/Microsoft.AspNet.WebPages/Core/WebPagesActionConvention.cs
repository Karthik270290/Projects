// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesDefaultActionConvention : Attribute,
                                                   IActionModelConvention,
                                                   IActionConstraintFactory
    {
        private IActionConstraint _constraint;

        public IActionConstraint CreateInstance(IServiceProvider serviceProvider)
        {
            if (_constraint == null)
            {
                var options = serviceProvider.GetService<IOptionsAccessor<WebPagesOptions>>();
                var path = options.Options.PagesFolderPath.TrimEnd(Coordinator.PathSeparators);

                var viewEngine = serviceProvider.GetService<ICompositeViewEngine>();
                _constraint = new WebPagesActionConstraint(viewEngine,
                    path.TrimEnd(Coordinator.PathSeparators));
            }

            return _constraint;
        }

        public void Apply([NotNull]ActionModel model)
        {
            model.ApiExplorerIsVisible = false;
        }
    }
}