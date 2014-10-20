// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages.Core
{
    // This class acts as a controller, but is named differently so it only gets exposed through
    // the WebPagesActionDescriptorProvider.
    public class PageCoordinator
    {
        [Activate]
        public ActionContext Context { get; set; }

        [Activate]
        public IOptions<WebPagesOptions> Options { get; set; }

        [Activate]
        public ICompositeViewEngine ViewEngine { get; set; }

        [Activate]
        public ViewDataDictionary ViewData { get; set; }

        [WebPagesDefaultActionConvention]
        public IActionResult WebPagesView(string __viewPath)
        {
            var viewPath = Options.Options.PagesFolderPath.TrimEnd(Coordinator.PathSeparators)
                + "/"
                + __viewPath;

            var result = ViewEngine.FindView(Context, viewPath);

            if (result.Success)
            {
                return new WebPagesViewResult(result.View, ViewData);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
    }
}
