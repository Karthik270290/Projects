// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;

namespace Microsoft.AspNet.WebPages.Core
{
    public class RouteTemplate : IRouteTemplateProvider
    {
        public RouteTemplate([NotNull] string route)
        {
            Template = route;
        }

        public string Name { get { return Guid.NewGuid().ToString(); } }

        public int? Order { get { return null; } }

        public string Template { get; private set; }
    }
}
