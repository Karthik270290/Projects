using System;
using System.Globalization;
using Microsoft.AspNet.Razor.Generator;
using Microsoft.AspNet.Razor.Parser.SyntaxTree;

namespace Microsoft.AspNet.Mvc.Razor.Host
{
    public class RouteCodeGenerator : SpanCodeGenerator
    {
        public RouteCodeGenerator(string route, string verb)
        {
            Route = route;
            Verb = verb;
        }

        public string Route { get; private set; }

        public string Verb { get; private set; }

        public override void GenerateCode(Span target, CodeGeneratorContext context)
        {
            var routeChunk = new RouteChunk(Route, Verb);
            context.CodeTreeBuilder.AddChunk(routeChunk, target);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "@route {0}", Route);
        }

        public override bool Equals(object obj)
        {
            var other = obj as RouteCodeGenerator;
            return other != null &&
                   string.Equals(Route, other.Route, StringComparison.Ordinal) &&
                   string.Equals(Verb, other.Verb, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return Route.GetHashCode() +
                   (Verb.GetHashCode() * 13);
        }
    }
}
