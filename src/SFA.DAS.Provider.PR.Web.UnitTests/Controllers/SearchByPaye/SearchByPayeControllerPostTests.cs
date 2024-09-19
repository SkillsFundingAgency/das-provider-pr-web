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

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByPaye;
public class SearchByPayeControllerPostTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = BackLink;
    private static readonly string AddEmployerContactDetails = Guid.NewGuid().ToString();

    private static readonly string Email = "test@account.com";

    [Test, MoqAutoData]
    public async Task Post_NotMultipleAccounts_ReturnsExpectedViewModelAndPath(
     Mock<IOuterApiClient> outerApiClientMock,
     Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
     Mock<ISessionService> sessionServiceMock,
     int ukprn,
     string paye,
     string aorn,
     GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
     CancellationToken cancellationToken
     )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        getRelationshipsByUkprnPayeAornResponse.HasActiveRequest = false;
        getRelationshipsByUkprnPayeAornResponse.HasOneLegalEntity = null;

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerSearchByPaye);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_OuterApiReturnNull_ReturnsExpectedViewModelAndPath(
  Mock<IOuterApiClient> outerApiClientMock,
  Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
  Mock<ISessionService> sessionServiceMock,
  int ukprn,
  string paye,
  string aorn,
  CancellationToken cancellationToken
  )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync((GetRelationshipsByUkprnPayeAornResponse)null!);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerContactDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_SessionModelNotFound_RedirectedToStart(
        Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
       )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public async Task Post_RedirectsToStartIfEmailNotSetInSession(
        Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
    )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }


    [Test, MoqAutoData]
    public async Task Post_Invalid_ReturnsExpectedViewModelAndPath(
        Mock<IOuterApiClient> outerApiClientMock,
        Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
        int ukprn,
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
    )
    {
        searchByPayeSubmitViewModel.Email = Email;

        validatorMock.Setup(m => m.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        SearchByPayeController sut = new(outerApiClientMock.Object, Mock.Of<ISessionService>(), validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByPayeViewModel? viewModel = viewResult.Model as SearchByPayeViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Paye.Should().Be(searchByPayeSubmitViewModel.Paye);
        viewModel.Aorn.Should().Be(searchByPayeSubmitViewModel.Aorn);
    }

    [Test, MoqAutoData]
    public async Task Post_HasActiveRequest_RedirectsToShutterPage(
        Mock<IOuterApiClient> outerApiClientMock,
        Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string paye,
        string aorn,
        CancellationToken cancellationToken
    )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse =
            new GetRelationshipsByUkprnPayeAornResponse
            {
                HasActiveRequest = true
            };

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerInvitationAlreadySent);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task Post_MultipleAccounts_RedirectsToShutterPage(
       Mock<IOuterApiClient> outerApiClientMock,
       Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
       Mock<ISessionService> sessionServiceMock,
       int ukprn,
       string paye,
       string aorn,
       GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
       CancellationToken cancellationToken
       )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        getRelationshipsByUkprnPayeAornResponse.HasActiveRequest = false;
        getRelationshipsByUkprnPayeAornResponse.HasOneLegalEntity = false;

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerMultipleAccounts);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_PayeAornNotCorrect_RedirectsToPayeAornNotFoundShutterPage(
       Mock<IOuterApiClient> outerApiClientMock,
       Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
       Mock<ISessionService> sessionServiceMock,
       int ukprn,
       string paye,
       string aorn,
       GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
       CancellationToken cancellationToken
       )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        getRelationshipsByUkprnPayeAornResponse.HasActiveRequest = false;
        getRelationshipsByUkprnPayeAornResponse.HasInvalidPaye = true;

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerPayeAornNotCorrect);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task Post_NotInEAS_RedirectsToAddContactDetails(
      Mock<IOuterApiClient> outerApiClientMock,
      Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
      Mock<ISessionService> sessionServiceMock,
      int ukprn,
      string paye,
      string aorn,
      GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
      CancellationToken cancellationToken
      )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        getRelationshipsByUkprnPayeAornResponse.HasActiveRequest = false;
        getRelationshipsByUkprnPayeAornResponse.HasOneLegalEntity = true;
        getRelationshipsByUkprnPayeAornResponse.Account = null;
        getRelationshipsByUkprnPayeAornResponse.Organisation = null;

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.AddEmployerContactDetails, AddEmployerContactDetails);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerContactDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_NotInEASOrganisationIsNull_RedirectsToAddContactDetails(
      Mock<IOuterApiClient> outerApiClientMock,
      Mock<IValidator<SearchByPayeSubmitViewModel>> validatorMock,
      Mock<ISessionService> sessionServiceMock,
      int ukprn,
      string paye,
      string aorn,
      GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
      string organisationName,
      CancellationToken cancellationToken
      )
    {
        SearchByPayeSubmitViewModel searchByPayeSubmitViewModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        getRelationshipsByUkprnPayeAornResponse.HasActiveRequest = false;
        getRelationshipsByUkprnPayeAornResponse.HasOneLegalEntity = true;
        getRelationshipsByUkprnPayeAornResponse.Account = null;
        getRelationshipsByUkprnPayeAornResponse.Organisation = new OrganisationDetails { Name = organisationName };

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitViewModel>())).Returns(new ValidationResult());

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.AddEmployerContactDetails, AddEmployerContactDetails);

        var result = await sut.Index(ukprn, searchByPayeSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerContactDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.OrganisationName == organisationName)), Times.AtLeastOnce);
    }
}
