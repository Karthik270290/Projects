// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages
{
    /// <summary>
    /// Sets up default options for <see cref="WebPagesOptions"/>.
    /// </summary>
    public class WebPagesOptionsSetup : OptionsAction<WebPagesOptions>
    {
        /// <remarks>
        /// Sets the Order to DefaultOrder to allow WebPagesOptionsSetup to run 
        /// before a user call to ConfigureOptions.
        /// </remarks>
        public WebPagesOptionsSetup() : base(ConfigureWebPages)
        {
            Order = DefaultOrder.DefaultFrameworkSortOrder;
        }

        /// <inheritdoc />
        public static void ConfigureWebPages(WebPagesOptions options)
        {
            options.PagesUrlPrefix = "";
            options.PagesFolderPath = "/Pages";

            options.RoutedPagesFolderPath = "/RoutedPages";
        }
    }
}
