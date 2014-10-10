// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.WebPages.Core;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages
{
    public class WebPagesServices
    {
        public static IEnumerable<IServiceDescriptor> GetDefaultServices()
        {
            return GetDefaultServices(new Configuration());
        }

        public static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Transient<IOptionsAction<WebPagesOptions>, WebPagesOptionsSetup>();

            yield return describe.Transient<INestedProvider<ActionDescriptorProviderContext>,
                                            WebPagesActionDescriptorProvider>();

            // For now using the global filters across the whole framework, but we can consider
            // splitting for webpages soon.
            // The IGlobalFilterProvider is used to build the action descriptors (likely once) and so should
            // remain transient to avoid keeping it in memory.
            // yield return describe.Transient<IGlobalFilterProvider, DefaultGlobalFilterProvider>();
        }
    }
}
