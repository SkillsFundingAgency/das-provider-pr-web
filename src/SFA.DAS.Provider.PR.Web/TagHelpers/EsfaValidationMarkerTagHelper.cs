﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.Provider.PR.Web.TagHelpers;

[ExcludeFromCodeCoverage]
[HtmlTargetElement("div", Attributes = ValidationForAttributeName)]
public class EsfaValidationMarkerTagHelper : TagHelper
{
    private const string ValidationForAttributeName = "esfa-validation-marker-for";
    private const string ClassAttributeIdentifier = "class";
    private const string ErrorClassSpecifier = "govuk-form-group--error";

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    [HtmlAttributeName(ValidationForAttributeName)]
    public ModelExpression For { get; set; } = null!;

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext.ModelState.TryGetValue(For.Name, out var modelStateEntry) && modelStateEntry.ValidationState == ModelValidationState.Invalid)
        {
            if (output.Attributes.ContainsName(ClassAttributeIdentifier))
            {
                output.Attributes.TryGetAttribute(ClassAttributeIdentifier, out var classAttr);
                output.Attributes.SetAttribute(ClassAttributeIdentifier, $"{classAttr.Value} {ErrorClassSpecifier}");
            }
            else
            {
                output.Attributes.SetAttribute(ClassAttributeIdentifier, ErrorClassSpecifier);
            }
        }

        return Task.CompletedTask;
    }
}
