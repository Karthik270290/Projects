// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages.Core
{
    // This class acts as a controller, but is named differently so it only gets exposed through
    // the WebPagesActionDescriptorProvider.
    public abstract class Coordinator
    {
        public static readonly char[] PathSeparators = new[] { '/', '\\' };
        public static readonly string ViewPathRouteKey = "__viewPath";

        [Activate]
        public ViewDataDictionary ViewData { get; set; }

        [Activate]
        public ICompositeViewEngine ViewEngine { get; set; }

        [Activate]
        public ActionContext ActionContext { get; set; }

        [Activate]
        public IOptionsAccessor<WebPagesOptions> OptionsAccessor { get; set; }
    }
}
