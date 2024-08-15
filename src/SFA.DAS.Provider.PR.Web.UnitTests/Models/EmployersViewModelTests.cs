using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class EmployersViewModelTests
{
    [Test, AutoData]
    public void Ctor_IntialisesObject(GetProviderRelationshipsResponse source, string clearFilterLink, string addEmployerLink, int ukprn)
    {
        Mock<IUrlHelper> urlHelperMock = new();
        urlHelperMock
            .AddUrlForRoute(RouteNames.Employers, clearFilterLink)
            .AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerLink);
        IEnumerable<EmployerPermissionViewModel> expected = source.Employers.Select(e => (EmployerPermissionViewModel)e);

        EmployersViewModel sut = new(source, urlHelperMock.Object, ukprn);

        using (new AssertionScope())
        {
            sut.TotalCount.Should().Be("employer".ToQuantity(source.TotalCount));
            sut.Employers.Should().BeEquivalentTo(expected);
            sut.ClearFiltersLink.Should().Be(clearFilterLink);
            sut.AddEmployerLink.Should().Be(addEmployerLink);
        }
    }
}
