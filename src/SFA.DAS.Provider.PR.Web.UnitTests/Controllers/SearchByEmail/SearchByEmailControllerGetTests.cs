using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class SearchByEmailControllerGetTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = BackLink;
    private static readonly string AddLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel(int ukprn)
    {
        SearchByEmailController sut = new(Mock.Of<IOuterApiClient>(), Mock.Of<ISessionService>(), Mock.Of<IValidator<SearchByEmailSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByEmailViewModel? viewModel = viewResult.Model as SearchByEmailViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Email.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void MultipleAccountsShutterPage_BuildsExpectedViewModel(int ukprn)
    {
        SearchByEmailController sut = new(Mock.Of<IOuterApiClient>(), Mock.Of<ISessionService>(), Mock.Of<IValidator<SearchByEmailSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddLink);

        var result = sut.MultipleAccountsShutterPage(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        MultipleAccountsShutterPageViewModel? viewModel = viewResult.Model as MultipleAccountsShutterPageViewModel;
        viewModel!.AddLink.Should().Be(AddLink);
    }
}
