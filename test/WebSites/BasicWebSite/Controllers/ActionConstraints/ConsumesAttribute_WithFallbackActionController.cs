// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicWebSite.Models;
using Microsoft.AspNet.Mvc;

namespace BasicWebSite.Controllers.ActionConstraints
{
    [Route("ConsumesAttribute_Company/[action]")]
    public class ConsumesAttribute_WithFallbackActionController : Controller
    {
        [Consumes("application/json")]
        public IActionResult CreateProduct(Product_Json jsonInput)
        {
            return Content("CreateProduct_Product_Json");
        }

        [Consumes("application/xml")]
        public IActionResult CreateProduct(Product_Xml xmlInput)
        {
            return Content("CreateProduct_Product_Xml");
        }

        public IActionResult CreateProduct(Product_Text defaultInput)
        {
            return Content("CreateProduct_Product_Text");
        }
    }
}