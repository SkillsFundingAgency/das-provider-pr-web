using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class SearchByEmailControllerPostTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = BackLink;
    private static readonly string RedirectToMultipleAccountsShutterPage = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public async Task Post_NoMultipleAccounts_ReturnsExpectedViewModelAndPath(
        Mock<IOuterApiClient> outerApiClientMock,
        Mock<IValidator<SearchByEmailSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        SearchByEmailSubmitViewModel searchByEmailSubmitViewModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
        )
    {
        var email = "test@account.com";
        searchByEmailSubmitViewModel.Email = email;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = true;
        getRelationshipByEmailResponse.HasOneLegalEntity = true;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(email, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByEmailSubmitViewModel>())).Returns(new ValidationResult());

        SearchByEmailController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByEmailSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerSearchByEmail);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetRelationshipByEmail(email, ukprn, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.Email == email)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_Invalid_ReturnsExpectedViewModelAndPath(
        Mock<IOuterApiClient> outerApiClientMock,
        Mock<IValidator<SearchByEmailSubmitViewModel>> validatorMock,
        int ukprn,
        SearchByEmailSubmitViewModel searchByEmailSubmitViewModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        var email = "test@account.com";
        searchByEmailSubmitViewModel.Email = email;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = true;
        getRelationshipByEmailResponse.HasOneLegalEntity = true;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(email, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(m => m.Validate(It.IsAny<SearchByEmailSubmitViewModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        SearchByEmailController sut = new(outerApiClientMock.Object, Mock.Of<ISessionService>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByEmailSubmitViewModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByEmailViewModel? viewModel = viewResult.Model as SearchByEmailViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Email.Should().Be(email);
    }

    [Test]
    [MoqInlineAutoData(true, false)]
    [MoqInlineAutoData(false, true)]
    [MoqInlineAutoData(false, false)]
    [MoqInlineAutoData(true, null)]
    [MoqInlineAutoData(false, null)]
    public async Task Post_MultipleAccounts_RedirectsToShutterPage(
        bool? hasOneEmployerAccount,
        bool? hasOneLegalEntity,
        Mock<IOuterApiClient> outerApiClientMock,
        Mock<IValidator<SearchByEmailSubmitViewModel>> validatorMock,
        int ukprn,
        SearchByEmailSubmitViewModel searchByEmailSubmitViewModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        var email = "test@account.com";
        searchByEmailSubmitViewModel.Email = email;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = hasOneEmployerAccount;
        getRelationshipByEmailResponse.HasOneLegalEntity = hasOneLegalEntity;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(email, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);
        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByEmailSubmitViewModel>())).Returns(new ValidationResult());

        SearchByEmailController sut = new(outerApiClientMock.Object, Mock.Of<ISessionService>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerMultipleAccounts, RedirectToMultipleAccountsShutterPage);

        var result = await sut.Index(ukprn, searchByEmailSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerMultipleAccounts);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task Post_HasAccountFalse_ReturnsExpectedViewModel(
        Mock<IOuterApiClient> outerApiClientMock,
        Mock<IValidator<SearchByEmailSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        SearchByEmailSubmitViewModel searchByEmailSubmitViewModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        var email = "test@account.com";
        searchByEmailSubmitViewModel.Email = email;
        getRelationshipByEmailResponse.HasUserAccount = false;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(email, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);
        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByEmailSubmitViewModel>())).Returns(new ValidationResult());

        SearchByEmailController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByEmailSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerSearchByEmail);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetRelationshipByEmail(email, ukprn, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.Email == email)), Times.Once);
    }
}
