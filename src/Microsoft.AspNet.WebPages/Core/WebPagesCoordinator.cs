using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

namespace Microsoft.AspNet.WebPages.Core
{
    // This class acts as a controller, but is named differently so it only gets exposed through
    // the WebPagesActionDescriptorProvider.
    public class WebPagesCoordinator
    {
        public readonly string ViewAttributeRouteFormatString = "{0}/{viewPath*}";

        [Activate]
        public ViewDataDictionary ViewData { get; set; }

        [Activate]
        public ICompositeViewEngine ViewEngine { get; set; }

        [Activate]
        public ActionContext ActionContext { get; set; }

        public IActionResult WebPagesView(string viewPath)
        {
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
    }
}
