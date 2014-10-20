// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesRoutedActionConvention : Attribute,
                                                  IActionModelConvention
    {
        public void Apply([NotNull] ActionModel model)
        {
            model.ApiExplorerIsVisible = false;
        }
    }
}
