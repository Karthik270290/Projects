#pragma checksum "TestFiles/Input/InjectWithSemicolon.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fc807ec0dc76610bdca62f482fefd7f584348df9"
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Threading.Tasks;
    public class TestFiles_Input_InjectWithSemicolon_cshtml : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MyModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
        }
        #pragma warning restore 1998
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
public MyService<MyModel> Html2 { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
public MyApp MyPropertyName2 { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
public MyService<MyModel> Html { get; private set; }
        [Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
public MyApp MyPropertyName { get; private set; }
    }
}
