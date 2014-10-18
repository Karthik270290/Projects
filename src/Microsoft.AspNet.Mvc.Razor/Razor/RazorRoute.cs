// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Razor;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RazorRoute
    {
        public string RouteTemplate { get; private set; }
        public string Verb { get; private set; }

        public RazorRoute([NotNull] string routeTemplate, [NotNull] string verb)
        {
            RouteTemplate = routeTemplate;
            Verb = verb;
        }

        public RazorRoute([NotNull]RouteChunk chunk) : this(chunk.RouteTemplate, chunk.Verb)
        {
        }
    }
}
