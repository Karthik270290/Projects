using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.WebPages.Core
{
    // This class acts as a controller, but is named differently so it only gets exposed through
    // the WebPagesActionDescriptorProvider.
    [WebPagesDefaultActionConvention]
    public class WebPagesCoordinator
    {
        private static readonly char[] PathSeparators = new[] { '/', '\\' };

        [Activate]
        public ViewDataDictionary ViewData { get; set; }

        [Activate]
        public ICompositeViewEngine ViewEngine { get; set; }

        [Activate]
        public ActionContext ActionContext { get; set; }

        [Activate]
        public IOptionsAccessor<WebPagesOptions> OptionsAccessor { get; set; }

        [WebPagesDefaultActionConvention]
        public IActionResult WebPagesView(string viewPath)
        {
            viewPath = OptionsAccessor.Options.PagesFolderPath.TrimEnd(PathSeparators)
                + "/"
                + viewPath
                + ".cshtml";

            var result = ViewEngine.FindView(ActionContext, viewPath);

            if (result.Success)
            {
                return new WebPagesViewResult(result.View, ViewData);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        private class WebPagesActionConstraint : IActionConstraint
        {
            private readonly string _constraintPath;

            public WebPagesActionConstraint([NotNull] string constraintPath)
            {
                _constraintPath = constraintPath;
            }

            public int Order { get { return -10000; } }

            public bool Accept([NotNull] ActionConstraintContext context)
            {
                var viewPath = _constraintPath
                    + "/"
                    + (string)context.RouteContext.RouteData.Values["viewPath"]
                    + ".cshtml";

                ActionContext actionContext = new ActionContext(context.RouteContext,
                                                          context.CurrentCandidate.Action);

                // need to resolve services here, as this object has a different lifetime than
                // the request
                var viewEngine = context.RouteContext.HttpContext.RequestServices.GetService<ICompositeViewEngine>();
                var result = viewEngine.FindView(actionContext, viewPath);

                if (result.Success)
                {
                    // need to measure if we should cache the view
                    return true;
                }

                return false;
            }
        }

        private class WebPagesDefaultActionConvention : Attribute,
                                                        IActionModelConvention,
                                                        IActionConstraintFactory
        {
            private IActionConstraint _constraint;

            public IActionConstraint CreateInstance(IServiceProvider serviceProvider)
            {
                if (_constraint == null)
                {
                    var options = serviceProvider.GetService<IOptionsAccessor<WebPagesOptions>>();
                    var path = options.Options.PagesFolderPath.TrimEnd(PathSeparators);

                    _constraint = new WebPagesActionConstraint(path.TrimEnd(PathSeparators));
                }

                return _constraint;
            }

            public void Apply([NotNull]ActionModel model)
            {
                model.ApiExplorerIsVisible = false;
            }
        }

        public class RouteTemplate : IRouteTemplateProvider
        {
            public readonly string _viewAttributeRouteFormatString = "{0}/{{*viewPath:minlength(1)}}";

            private string _urlPrefix;

            public RouteTemplate([NotNull] string urlPrefix)
            {
                _urlPrefix = urlPrefix;
            }

            public string Name { get { return "__WebPages__"; } }

            public int? Order { get { return null; } }

            public string Template
            {
                get
                {
                    return string.Format(_viewAttributeRouteFormatString, _urlPrefix.Trim(PathSeparators));
                }
            }
        }
    }
}
