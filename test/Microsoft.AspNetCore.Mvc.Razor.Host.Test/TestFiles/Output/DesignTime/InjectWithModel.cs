namespace AspNetCore
{
    #line hidden
    using System;
    using System.Threading.Tasks;
    public class testfiles_input_injectwithmodel_cshtml : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MyModel>
    {
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        ((System.Action)(() => {
MyModel __typeHelper = null;
        }
        ))();
        ((System.Action)(() => {
MyApp __typeHelper = null;
        }
        ))();
        ((System.Action)(() => {
System.Object MyPropertyName = null;
        }
        ))();
        ((System.Action)(() => {
MyService<TModel> __typeHelper = null;
        }
        ))();
        ((System.Action)(() => {
System.Object Html = null;
        }
        ))();
        }
        #pragma warning restore 219
        private static System.Object __o = null;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
        }
        #pragma warning restore 1998
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
public MyService<MyModel> Html { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
public MyApp MyPropertyName { get; private set; }
    }
}
