// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.WebPages;
using Microsoft.Framework.ConfigurationModel;

namespace Microsoft.Framework.DependencyInjection
{
    public static class MvcServiceCollectionExtensions
    {
        public static IServiceCollection AddWebPages(this IServiceCollection services)
        {
            return services.Add(WebPagesServices.GetDefaultServices());
        }

        public static IServiceCollection AddWebPages(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.Add(WebPagesServices.GetDefaultServices(configuration));
        }
    }
}
