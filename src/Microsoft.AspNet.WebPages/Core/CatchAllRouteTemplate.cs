// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;

namespace Microsoft.AspNet.WebPages.Core
{
    public class CatchAllRouteTemplate : IRouteTemplateProvider
    {
        public readonly string _viewAttributeRouteFormatString = "{0}/{{*__viewPath:minlength(1)}}";

        private string _urlPrefix;

        public CatchAllRouteTemplate([NotNull] string urlPrefix)
        {
            _urlPrefix = urlPrefix;
        }

        public string Name { get { return "__WebPages__"; } }

        public int? Order { get { return null; } }

        public string Template
        {
            get
            {
                return string.Format(_viewAttributeRouteFormatString, 
                    _urlPrefix.Trim(Coordinator.PathSeparators));
            }
        }
    }
}
