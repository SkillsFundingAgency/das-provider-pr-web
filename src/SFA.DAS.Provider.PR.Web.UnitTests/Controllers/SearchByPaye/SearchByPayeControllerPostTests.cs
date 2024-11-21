using AutoFixture.NUnit3;
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
    public async Task Post_SessionModelNotFound_RedirectedToStart(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
       )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public async Task Post_RedirectsToStartIfEmailNotSetInSession(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
    )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
        {
            Email = Email,
            Aorn = aorn,
            Paye = paye
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }


    [Test, MoqAutoData]
    public async Task Post_Invalid_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        SearchByPayeSubmitModel searchByPayeSubmitModel,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
    )
    {
        searchByPayeSubmitModel.Email = Email;

        validatorMock.Setup(m => m.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByPayeModel? viewModel = viewResult.Model as SearchByPayeModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Paye.Should().Be(searchByPayeSubmitModel.Paye);
        viewModel.Aorn.Should().Be(searchByPayeSubmitModel.Aorn);
    }

    [Test, MoqAutoData]
    public async Task Post_Invalid_PayeAndAornTrimmed(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        SearchByPayeSubmitModel searchByPayeSubmitModel,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
    )
    {
        searchByPayeSubmitModel.Email = Email;
        searchByPayeSubmitModel.Paye = $" {paye}";
        searchByPayeSubmitModel.Aorn = $"{aorn} ";

        validatorMock.Setup(m => m.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByPayeModel? viewModel = viewResult.Model as SearchByPayeModel;

        viewModel!.Paye.Should().Be(paye);
        viewModel.Aorn.Should().Be(aorn);
    }

    [Test, MoqAutoData]
    public async Task Post_HasActiveRequest_RedirectsToShutterPage(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        CancellationToken cancellationToken
    )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerInvitationAlreadySent);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task Post_HasActiveRequest_PayeAndAornTrimmed(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        CancellationToken cancellationToken
    )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
        {
            Email = Email,
            Aorn = $" {aorn}",
            Paye = $"{paye} "
        };

        var encodedPaye = Uri.EscapeDataString(paye);

        var email = "test@test.com";

        var sessionModel = new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse =
            new GetRelationshipsByUkprnPayeAornResponse
            {
                HasActiveRequest = true
            };

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        sessionServiceMock.Verify(x => x.Set(It.Is<AddEmployerSessionModel>
            (x => x.Aorn == aorn)), Times.Exactly(2));
        sessionServiceMock.Verify(x => x.Set(It.Is<AddEmployerSessionModel>
            (x => x.Paye == paye)), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public async Task Post_MultipleAccounts_RedirectsToShutterPage(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
       )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerMultipleAccounts);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_PayeAornNotCorrect_RedirectsToPayeAornNotFoundShutterPage(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
       )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerPayeAornNotCorrect);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task Post_NotInEAS_RedirectsToAddContactDetails(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        CancellationToken cancellationToken
      )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.AddEmployerContactDetails, AddEmployerContactDetails);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerContactDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_NotInEASOrganisationIsNull_RedirectsToAddContactDetails(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        string organisationName,
        CancellationToken cancellationToken
      )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.AddEmployerContactDetails, AddEmployerContactDetails);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerContactDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.OrganisationName == organisationName)), Times.AtLeastOnce);
    }

    [Test]
    [MoqInlineAutoData(null, null, null)]
    [MoqInlineAutoData(5L, "LE Name", "org name")]

    public async Task Post_PayeAndAornLinkedEmailNotMatched_RedirectsToExpectedShutterPage(
        long? accountLegalEntityId,
        string accountLegalEntityName,
        string? organisationName,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
        AccountDetails? accountDetails,
        CancellationToken cancellationToken
      )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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
        getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityId = accountLegalEntityId;
        getRelationshipsByUkprnPayeAornResponse.Account = accountDetails;
        getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityName = accountLegalEntityName;
        getRelationshipsByUkprnPayeAornResponse.Organisation = new OrganisationDetails { Name = organisationName };

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.AddEmployerContactDetails, AddEmployerContactDetails);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.PayeAornMatchedEmailNotLinkedNoRelationshipLink);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        var expectedAccountId = getRelationshipsByUkprnPayeAornResponse.Account?.AccountId;

        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(
            x => x.OrganisationName == organisationName
            && x.AccountLegalEntityId == getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityId
            && x.AccountLegalEntityName == getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityName
            && x.OrganisationName == getRelationshipsByUkprnPayeAornResponse.Organisation.Name
        )), Times.AtLeastOnce);
    }

    [Test, MoqAutoData]
    public async Task Post_PayeAndAornLinkedEmailNotMatchedHasRelationship_RedirectsToExpectedShutterPage(
       long? accountLegalEntityId,
       string accountLegalEntityName,
       string? organisationName,
       [Frozen] Mock<IOuterApiClient> outerApiClientMock,
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
       [Greedy] SearchByPayeController sut,
       int ukprn,
       string paye,
       string aorn,
       GetRelationshipsByUkprnPayeAornResponse getRelationshipsByUkprnPayeAornResponse,
       AccountDetails? accountDetails,
       CancellationToken cancellationToken
     )
    {
        SearchByPayeSubmitModel searchByPayeSubmitModel = new()
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
        getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityId = accountLegalEntityId;
        getRelationshipsByUkprnPayeAornResponse.Account = accountDetails;
        getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityName = accountLegalEntityName;
        getRelationshipsByUkprnPayeAornResponse.Organisation = new OrganisationDetails { Name = organisationName };
        getRelationshipsByUkprnPayeAornResponse.HasRelationship = true;

        outerApiClientMock.Setup(x => x.GetProviderRelationshipsByUkprnPayeAorn(ukprn, aorn, encodedPaye, cancellationToken)).ReturnsAsync(getRelationshipsByUkprnPayeAornResponse);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByPayeSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.AddEmployerContactDetails, AddEmployerContactDetails);

        var result = await sut.Index(ukprn, searchByPayeSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.PayeAornMatchedEmailNotLinkedHasRelationshipLink);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        var expectedAccountId = getRelationshipsByUkprnPayeAornResponse.Account?.AccountId;

        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(
            x => x.OrganisationName == organisationName
            && x.AccountLegalEntityId == getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityId
            && x.AccountLegalEntityName == getRelationshipsByUkprnPayeAornResponse.AccountLegalEntityName
            && x.OrganisationName == getRelationshipsByUkprnPayeAornResponse.Organisation.Name
        )), Times.AtLeastOnce);
    }
}