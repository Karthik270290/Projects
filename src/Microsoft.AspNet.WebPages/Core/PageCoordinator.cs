// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace Microsoft.AspNet.WebPages.Core
{
    public class PageCoordinator : Coordinator
    {        
        [WebPagesDefaultActionConvention]
        public IActionResult WebPagesView(string __viewPath)
        {
            var viewPath = OptionsAccessor.Options.PagesFolderPath.TrimEnd(PathSeparators)
                + "/"
                + __viewPath;

            var result = ViewEngine.FindView(ActionContext, viewPath);

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
