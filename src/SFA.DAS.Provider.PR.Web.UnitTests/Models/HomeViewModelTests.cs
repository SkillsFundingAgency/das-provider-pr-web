using AutoFixture.NUnit3;
using FluentAssertions;
using Humanizer;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class HomeViewModelTests
{
    [Test, AutoData]
    public void Ctor_IntialisesObject(GetProviderRelationshipsResponse source, string clearFilterLink)
    {
        IEnumerable<EmployerPermissionViewModel> expected = source.Employers.Select(e => (EmployerPermissionViewModel)e);
        HomeViewModel sut = new(source, clearFilterLink);

        sut.TotalCount.Should().Be("employer".ToQuantity(source.TotalCount));
        sut.Employers.Should().BeEquivalentTo(expected);
        sut.ClearFiltersLink.Should().Be(clearFilterLink);
    }
}
