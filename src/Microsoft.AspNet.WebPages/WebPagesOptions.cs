// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.WebPages
{
    public class WebPagesOptions
    {
        private string _pagesFolderName;
        private string _pagesFolderPath;

        private string _routedPagesFolderName;
        private string _routedPagesFolderPath;

        /// <summary>
        /// Path has to start with / or ~/
        /// </summary>
        public string PagesFolderName
        {
            get
            {
                return _pagesFolderName;
            }

            set
            {
                _pagesFolderPath = ToPath(value);
                _pagesFolderName = value;
            }
        }

        public string PagesFolderPath => _pagesFolderPath;

        /// <summary>
        /// Path has to start with / or ~/
        /// </summary>
        public string RoutedPagesFolderName
        {
            get
            {
                return _routedPagesFolderName;
            }

            set
            {
                _routedPagesFolderPath = ToPath(value);
                _routedPagesFolderName = value;
            }
        }


        public string RoutedPagesFolderPath => _routedPagesFolderPath;

        private static string ToPath(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return "~/" + name.Trim(new[] { '~', '/', '\\' }) + "/";
        }
    }
}