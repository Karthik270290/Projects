namespace AspNetCore
{
    #line hidden
    using System;
    using System.Threading.Tasks;
    public class testfiles_input_modelexpressiontaghelper_cshtml : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DateTime>
    {
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        ((System.Action)(() => {
DateTime __typeHelper = null;
        }
        ))();
        ((System.Action)(() => {
System.Object __typeHelper = "Microsoft.AspNetCore.Mvc.Razor.InputTestTagHelper, Microsoft.AspNetCore.Mvc.Razor.Host.Test";
        }
        ))();
        ((System.Action)(() => {
System.Object __typeHelper = "Microsoft.AspNetCore.Mvc.Razor.DictionaryPrefixTestTagHelper, Microsoft.AspNetCore.Mvc.Razor.Host.Test";
        }
        ))();
        }
        #pragma warning restore 219
        private static System.Object __o = null;
        private global::Microsoft.AspNetCore.Mvc.Razor.InputTestTagHelper __Microsoft_AspNetCore_Mvc_Razor_InputTestTagHelper = null;
        private global::Microsoft.AspNetCore.Mvc.Razor.DictionaryPrefixTestTagHelper __Microsoft_AspNetCore_Mvc_Razor_DictionaryPrefixTestTagHelper = null;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            __Microsoft_AspNetCore_Mvc_Razor_InputTestTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.InputTestTagHelper>();
#line 6 "testfiles/input/modelexpressiontaghelper.cshtml"
__Microsoft_AspNetCore_Mvc_Razor_InputTestTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Now);

#line default
#line hidden
            __Microsoft_AspNetCore_Mvc_Razor_InputTestTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.InputTestTagHelper>();
#line 7 "testfiles/input/modelexpressiontaghelper.cshtml"
__Microsoft_AspNetCore_Mvc_Razor_InputTestTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => Model);

#line default
#line hidden
            __Microsoft_AspNetCore_Mvc_Razor_DictionaryPrefixTestTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.DictionaryPrefixTestTagHelper>();
#line 9 "testfiles/input/modelexpressiontaghelper.cshtml"
__Microsoft_AspNetCore_Mvc_Razor_DictionaryPrefixTestTagHelper.PrefixValues["test"] = ModelExpressionProvider.CreateModelExpression(ViewData, __model => Model);

#line default
#line hidden
            __Microsoft_AspNetCore_Mvc_Razor_DictionaryPrefixTestTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.DictionaryPrefixTestTagHelper>();
#line 10 "testfiles/input/modelexpressiontaghelper.cshtml"
__Microsoft_AspNetCore_Mvc_Razor_DictionaryPrefixTestTagHelper.PrefixValues["hour"] = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Hour);

#line default
#line hidden
#line 10 "testfiles/input/modelexpressiontaghelper.cshtml"
__Microsoft_AspNetCore_Mvc_Razor_DictionaryPrefixTestTagHelper.PrefixValues["minute"] = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Minute);

#line default
#line hidden
        }
        #pragma warning restore 1998
    }
}
