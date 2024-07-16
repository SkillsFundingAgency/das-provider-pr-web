using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;
public class StartViewModelTests
{
    [Test, AutoData]
    public void Operator_ConvertsTo_StartViewModel(string continueLink, string viewEmployersAndPermissionsLink)
    {
        StartViewModel sut = new StartViewModel(continueLink, viewEmployersAndPermissionsLink);
        sut.ContinueLink.Should().Be(continueLink);
        sut.ViewEmployersAndPermissionsLink.Should().Be(viewEmployersAndPermissionsLink);
    }
}
