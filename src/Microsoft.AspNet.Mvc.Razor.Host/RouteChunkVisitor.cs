// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Razor.Generator;
using Microsoft.AspNet.Razor.Generator.Compiler.CSharp;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RouteChunkVisitor : MvcCSharpCodeVisitor
    {
        public RouteChunkVisitor([NotNull] CSharpCodeWriter writer,
                                 [NotNull] CodeBuilderContext context)
            : base(writer, context)
        {
        }

        public List<RouteChunk> RouteChunks { get; private set; } = new List<RouteChunk>();

        protected override void Visit([NotNull] RouteChunk chunk)
        {
            RouteChunks.Add(chunk);
        }
    }
}
