#pragma checksum "D:\DoAn\APIProj\Views\UserModels\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0e007e740670126ae55cc4fdc53eec00bd70cd54"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_UserModels_Edit), @"mvc.1.0.view", @"/Views/UserModels/Edit.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0e007e740670126ae55cc4fdc53eec00bd70cd54", @"/Views/UserModels/Edit.cshtml")]
    #nullable restore
    public class Views_UserModels_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<APIProj.Models.UserModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\DoAn\APIProj\Views\UserModels\Edit.cshtml"
  
    ViewData["Title"] = "Edit";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<h1>Edit</h1>

<h4>UserModel</h4>
<hr />
<div class=""row"">
    <div class=""col-md-4"">
        <form asp-action=""Edit"">
            <div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
            <input type=""hidden"" asp-for=""Id"" />
            <div class=""form-group"">
                <label asp-for=""UserName"" class=""control-label""></label>
                <input asp-for=""UserName"" class=""form-control"" />
                <span asp-validation-for=""UserName"" class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <label asp-for=""Password"" class=""control-label""></label>
                <input asp-for=""Password"" class=""form-control"" />
                <span asp-validation-for=""Password"" class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <label asp-for=""EmailAddress"" class=""control-label""></label>
                <input asp-for=""EmailAddress"" class=""form-control"" />
                <sp");
            WriteLiteral(@"an asp-validation-for=""EmailAddress"" class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <label asp-for=""Role"" class=""control-label""></label>
                <input asp-for=""Role"" class=""form-control"" />
                <span asp-validation-for=""Role"" class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <label asp-for=""SurName"" class=""control-label""></label>
                <input asp-for=""SurName"" class=""form-control"" />
                <span asp-validation-for=""SurName"" class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <label asp-for=""GiveName"" class=""control-label""></label>
                <input asp-for=""GiveName"" class=""form-control"" />
                <span asp-validation-for=""GiveName"" class=""text-danger""></span>
            </div>
            <div class=""form-group"">
                <input type=""submit"" value=""Save"" class=""btn btn-primary"" /");
            WriteLiteral(">\r\n            </div>\r\n        </form>\r\n    </div>\r\n</div>\r\n\r\n<div>\r\n    <a asp-action=\"Index\">Back to List</a>\r\n</div>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 58 "D:\DoAn\APIProj\Views\UserModels\Edit.cshtml"
      await Html.RenderPartialAsync("_ValidationScriptsPartial");

#line default
#line hidden
#nullable disable
            }
            );
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<APIProj.Models.UserModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
