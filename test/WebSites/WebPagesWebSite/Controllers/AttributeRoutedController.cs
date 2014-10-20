// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace WebPagesWebSite
{
    [WebSitesActionFilter]
    public class AttributeRoutedController
    {
        public const string Route = "/Attributed/Normal";
        public static string Response { get { return "Normal Attributed";} }

        [Activate]
        public ActionContext Context { get; set; }

        [Route(Route)]
        public string NormalAttributedAction()
        {
            if ((string)Context.RouteData.Values["filterData"] == "DataFromFilter")
            {
                return Response;
            }
            else
            {
                return "Filter did not run";
            }
        }
    }
}
