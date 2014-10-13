// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Razor;

namespace Microsoft.AspNet.Mvc.Razor
{
    public static class RazorRoute
    {
        public static IEnumerable<string> GetRoutes([NotNull] GeneratorResults generatorResults)
        {
            var chunks = generatorResults.CodeTree.Chunks
                            .OfType<RouteChunk>();

            // TODO: Expose verbs
            return chunks.Select(chunk => chunk.RouteTemplate);
        }

        public static IEnumerable<string> GetRoutes([NotNull] IMvcRazorHost host, [NotNull] RelativeFileInfo fileInfo)
        {
            using (var stream = fileInfo.FileInfo.CreateReadStream())
            {
                var results = host.GenerateCode(fileInfo.RelativePath, stream);
                return GetRoutes(results);
            }
        }
    }
}
