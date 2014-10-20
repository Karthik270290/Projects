// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Razor;

namespace Microsoft.AspNet.Mvc.Razor
{
    public static class RazorRoutes
    {
        public static IEnumerable<RazorRoute> GetRoutes([NotNull] GeneratorResults generatorResults)
        {
            var routes = generatorResults.CodeTree.Chunks
                            .OfType<RouteChunk>()
                            .Select(c => new RazorRoute(c));

            return routes;
        }

        // TODO: Should we instead compile so we can get the attributes out (for full support of filters?)
        public static IEnumerable<RazorRoute> GetRoutes([NotNull] IMvcRazorHost host, [NotNull] RelativeFileInfo fileInfo)
        {
            using (var stream = fileInfo.FileInfo.CreateReadStream())
            {
                var results = host.GenerateCode(fileInfo.RelativePath, stream);
                return GetRoutes(results);
            }
        }
    }
}
