using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.Rendering;

namespace Microsoft.AspNet.WebPages.Core
{
    public class WebPagesViewResult : IActionResult
    {
        private readonly IView _view;
        private readonly ViewDataDictionary _viewData;

        public WebPagesViewResult([NotNull] IView view, [NotNull] ViewDataDictionary viewData)
        {
            _view = view;
            _viewData = viewData;
        }

        public Task ExecuteResultAsync([NotNull] ActionContext context)
        {
            return ViewExecuter.ExecuteView(_view, context, _viewData);
        }
    }
}