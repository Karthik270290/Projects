// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.WebPages
{
    public class WebPagesOptions
    {
        private string _pagesUrlPrefix = string.Empty;
        private string _pagesFolderPath = string.Empty;
        private string _routedPagesFolderPath = string.Empty;

        public string PagesUrlPrefix
        {
            get
            {
                return _pagesUrlPrefix;
            }
            set
            {
                _pagesUrlPrefix = value ?? string.Empty;
            }
        }

        public string PagesFolderPath
        {
            get
            {
                return _pagesFolderPath;
            }
            set
            {
                _pagesFolderPath = value ?? string.Empty;
            }
        }

        public string RoutedPagesFolderPath
        {
            get
            {
                return _routedPagesFolderPath;
            }
            set
            {
                _routedPagesFolderPath = value ?? string.Empty;
            }
        }

        public bool UpdateRoutesFromPrecompilationAtStartup { get; set; } = true;
    }
}
