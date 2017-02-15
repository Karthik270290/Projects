#pragma checksum "TestFiles/Input/Basic.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "54a70ff4c6d27ac6cdc6725cb6bab12012015729"
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Threading.Tasks;
    public class TestFiles_Input_Basic_cshtml : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 4, true);
            WriteLiteral("<div");
            EndContext();
            BeginWriteAttribute("class", " class=\"", 4, "\"", 17, 1);
#line 1 "TestFiles/Input/Basic.cshtml"
WriteAttributeValue("", 12, logo, 12, 5, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(18, 24, true);
            WriteLiteral(">\r\n    Hello world\r\n    ");
            EndContext();
            BeginContext(43, 21, false);
#line 3 "TestFiles/Input/Basic.cshtml"
Write(Html.Input("SomeKey"));

#line default
#line hidden
            EndContext();
            BeginContext(64, 8, true);
            WriteLiteral("\r\n</div>");
            EndContext();
        }
        #pragma warning restore 1998
    }
}
