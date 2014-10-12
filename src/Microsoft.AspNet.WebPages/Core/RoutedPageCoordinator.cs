// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace Microsoft.AspNet.WebPages.Core
{
    // This class acts as a controller, but is named differently so it only gets exposed through
    // the WebPagesActionDescriptorProvider.
    public class RoutedPageCoordinator : Coordinator
    {
        [WebPagesDefaultActionConvention]
        public IActionResult WebPagesView(string __viewPath)
        {
            var result = ViewEngine.FindView(ActionContext, __viewPath);

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
