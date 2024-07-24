using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.Provider.PR.Web.TagHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.TagHelpers;
public class ShowTagHelperTests
{
    [Test]
    public void ShowTagHelper_False_SupressesOutput()
    {
        ShowTagHelper sut = new() { AspShow = false };

        var context = new TagHelperContext(
                tagName: "bold",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

        var output = new TagHelperOutput(
            "bold",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Hello, World!");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        sut.Process(context, output);

        output.IsContentModified.Should().BeTrue();
    }

    [Test]
    public void ShowTagHelper_True_RendersOutput()
    {
        ShowTagHelper sut = new() { AspShow = true };

        var context = new TagHelperContext(
                tagName: "bold",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object>(),
                uniqueId: "test");

        var output = new TagHelperOutput(
            "bold",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Hello, World!");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        sut.Process(context, output);

        output.IsContentModified.Should().BeFalse();
    }
}
