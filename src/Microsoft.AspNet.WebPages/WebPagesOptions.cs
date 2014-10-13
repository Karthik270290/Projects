// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.WebPages
{
    public class WebPagesOptions
    {
        public string PagesUrlPrefix { get; set; }

        public string PagesFolderPath { get; set; }

        public string RoutedPagesFolderPath { get; set; }

        public bool UpdateRoutesFromPrecompilationAtStartup { get; set; } = true;
    }
}
