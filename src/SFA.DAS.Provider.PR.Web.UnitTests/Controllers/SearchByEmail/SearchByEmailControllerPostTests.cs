﻿using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SFA.DAS.Encoding;
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
    private static readonly string RedirectToMultipleAccountsShutterPage = Guid.NewGuid().ToString();
    private static readonly string EmailSearchInviteAlreadySentPage = Guid.NewGuid().ToString();
    private const string ValidationError = "Validation Erorr";
    private const string ValidationErrorCode = "1001";
    private static readonly string Email = "test@account.com";
    private readonly string _emailCallingRelationships = Email;

    [Test, MoqAutoData]
    public async Task Post_SingleAccountsHasRelationship_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
     )
    {
        searchByEmailSubmitModel.Email = Email;

        getRelationshipByEmailResponse.HasActiveRequest = false;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = true;
        getRelationshipByEmailResponse.HasOneLegalEntity = true;
        getRelationshipByEmailResponse.HasRelationship = true;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.EmailLinkedToAccountWithRelationship);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.Email == Email)), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public async Task Post_HasActiveRequest_ReturnsExpectedViewModelAndPath(
      [Frozen] Mock<IOuterApiClient> outerApiClientMock,
      [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
      [Frozen] Mock<ISessionService> sessionServiceMock,
      [Greedy] SearchByEmailController sut,
      int ukprn,
      SearchByEmailSubmitModel searchByEmailSubmitModel,
      GetRelationshipByEmailResponse getRelationshipByEmailResponse,
      CancellationToken cancellationToken
  )
    {

        searchByEmailSubmitModel.Email = Email;
        getRelationshipByEmailResponse.HasActiveRequest = true;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.EmailSearchInviteAlreadySent);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task Post_HasActiveRequest_EmailIsTrimmedInSessionModel(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = $" {Email} ";
        getRelationshipByEmailResponse.HasActiveRequest = true;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var sut = new SearchByEmailController(outerApiClientMock.Object, sessionServiceMock.Object,
            encodingServiceMock.Object, validatorMock.Object);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        sessionServiceMock.Verify(x => x.Set(It.Is<AddEmployerSessionModel>(x => x.Email == Email)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_SingleAccountsHasNoRelationship_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = Email;
        getRelationshipByEmailResponse.HasActiveRequest = false;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = true;
        getRelationshipByEmailResponse.HasOneLegalEntity = true;
        getRelationshipByEmailResponse.HasRelationship = false;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.OneAccountNoRelationship);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.Email == Email)), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(
            x => x.Email == Email
                 && x.AccountId == getRelationshipByEmailResponse.AccountId
                 && x.AccountLegalEntityId == getRelationshipByEmailResponse.AccountLegalEntityId
                 && x.AccountLegalEntityName == getRelationshipByEmailResponse.AccountLegalEntityName
                 && x.Paye == getRelationshipByEmailResponse.Paye
            )), Times.AtLeastOnce);
    }

    [Test, MoqAutoData]
    public async Task Post_Invalid_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = Email;
        getRelationshipByEmailResponse.HasActiveRequest = false;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = true;
        getRelationshipByEmailResponse.HasOneLegalEntity = true;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);

        validatorMock.Setup(m => m.Validate(It.IsAny<SearchByEmailSubmitModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField", ValidationError) { ErrorCode = ValidationErrorCode }
        }));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByEmailModel? viewModel = viewResult.Model as SearchByEmailModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.Email.Should().Be(Email);
    }

    [Test, MoqAutoData]
    public async Task Post_EmailIsNull_ReturnsValidationErrorOnEmailProperty(
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = null;

        var validationFailures = new List<ValidationFailure>
        {
            new(nameof(searchByEmailSubmitModel.Email), ValidationError) { ErrorCode = ValidationErrorCode }
        };

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(validationFailures));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        var viewResult = result.As<ViewResult>();
        Assert.That(viewResult, Is.Not.Null);

        var viewModel = viewResult.Model as SearchByEmailModel;
        Assert.That(viewModel, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.Email, Is.Null);

            var emailError = viewResult.ViewData.ModelState[nameof(searchByEmailSubmitModel.Email)]?.Errors.FirstOrDefault();
            Assert.That(emailError, Is.Not.Null, "Expected a validation error for the 'Email' field.");
            Assert.That(emailError!.ErrorMessage, Is.EqualTo(ValidationError));
        });
    }

    [Test, MoqAutoData]
    public async Task Post_EmailIsEmpty_ReturnsValidationErrorOnEmailProperty(
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = "  ";

        var validationFailures = new List<ValidationFailure>
        {
            new(nameof(searchByEmailSubmitModel.Email), ValidationError) { ErrorCode = "1001" }
        };

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(validationFailures));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        var viewResult = result.As<ViewResult>();
        Assert.That(viewResult, Is.Not.Null);

        var viewModel = viewResult.Model as SearchByEmailModel;
        Assert.That(viewModel, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(viewModel!.Email, Is.EqualTo(searchByEmailSubmitModel.Email));

            var emailError = viewResult.ViewData.ModelState[nameof(searchByEmailSubmitModel.Email)]?.Errors.FirstOrDefault();
            Assert.That(emailError, Is.Not.Null, "Expected a validation error for the 'Email' field.");
            Assert.That(emailError!.ErrorMessage, Is.EqualTo(ValidationError));
        });
    }

    [Test, MoqAutoData]
    public async Task Post_EmailContainsWhiteSpace_EmailIsTrimmed(
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        CancellationToken cancellationToken
    )
    {
        string testEmail = " test@email.com ";
        searchByEmailSubmitModel.Email = testEmail;

        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        sessionServiceMock.Verify(x => x.Set(It.Is<AddEmployerSessionModel>(x => x.Email == testEmail.Trim())), Times.Once);
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
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = Email;
        getRelationshipByEmailResponse.HasActiveRequest = false;
        getRelationshipByEmailResponse.HasUserAccount = true;
        getRelationshipByEmailResponse.HasOneEmployerAccount = hasOneEmployerAccount;
        getRelationshipByEmailResponse.HasOneLegalEntity = hasOneLegalEntity;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerMultipleAccounts, RedirectToMultipleAccountsShutterPage);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerMultipleAccounts);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task Post_HasAccountFalse_RedirectsToSearchByPaye(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        SearchByEmailSubmitModel searchByEmailSubmitModel,
        GetRelationshipByEmailResponse getRelationshipByEmailResponse,
        CancellationToken cancellationToken
    )
    {
        searchByEmailSubmitModel.Email = Email;
        getRelationshipByEmailResponse.HasActiveRequest = false;
        getRelationshipByEmailResponse.HasUserAccount = false;

        outerApiClientMock.Setup(x => x.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken)).ReturnsAsync(getRelationshipByEmailResponse);
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SearchByEmailSubmitModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var result = await sut.Index(ukprn, searchByEmailSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerSearchByPaye);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        outerApiClientMock.Verify(o => o.GetRelationshipByEmail(_emailCallingRelationships, ukprn, cancellationToken), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(x => x.Email == Email)), Times.Once);
    }
}
