// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Localization;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNet.Mvc.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-loc", TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement("*", Attributes = "asp-loc-*", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class LocalizationTagHelper : TagHelper
    {
        //[HtmlAttributeName("asp-loc-attributes", DictionaryAttributePrefix = "asp-loc-")]
        //public IDictionary<string, bool> LocalizedAttributes { get; } = new Dictionary<string, bool>();

        [HtmlAttributeName("asp-localizer")]
        public IHtmlLocalizer Localizer { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var localizer = Localizer ?? ViewContext.HttpContext.RequestServices.GetService<IHtmlLocalizer>();

            var aspLocAttr = context.AllAttributes["asp-loc"];
            
            if (aspLocAttr != null)
            {
                var resourceKey = aspLocAttr.Minimized
                    ? (await output.GetChildContentAsync()).ToString()
                    : aspLocAttr.Value.ToString();
                output.Content.SetContent(Localizer.Html(resourceKey));
            }

            var localizeAttributes = context.AllAttributes.Where(attr => attr.Name.StartsWith("asp-loc-", System.StringComparison.OrdinalIgnoreCase));

            foreach (var attribute in localizeAttributes)
            {
                var attributeToLocalize = output.Attributes[attribute.Name.Substring("asp-loc-".Length)];
                if (attributeToLocalize != null)
                {
                    var resourceKey = attribute.Minimized
                        ? attributeToLocalize.Value.ToString()
                        : attribute.Value.ToString();
                    attributeToLocalize.Value = Localizer.Html(resourceKey);
                }
            }
        }
    }
}
