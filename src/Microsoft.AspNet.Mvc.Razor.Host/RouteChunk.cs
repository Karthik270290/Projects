using Microsoft.AspNet.Razor.Generator.Compiler;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RouteChunk : Chunk
    {
        public RouteChunk(string routeTemplate, string verb)
        {
            RouteTemplate = routeTemplate;
            Verb = verb;
        }

        public string RouteTemplate { get; set; }

        public string Verb { get; set; }
    }
}