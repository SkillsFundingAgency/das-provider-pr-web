using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;

public class EmailLinkedToAccountWithRelationshipShutterPageTests
{
    private static readonly string EmployerDetailsLink = Guid.NewGuid().ToString();
    private static readonly string AddEmployerStartLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void ShutterPage_BuildsExpectedViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        int accountLegalEntityId,
        string employerName)
    {
        var email = "test@test.com";
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = employerName
        });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetails, EmployerDetailsLink);

        var result = sut.EmailLinkedToAccountWithRelationshipShutterPage(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        EmailLinkedToAccountWithRelationshipShutterPageViewModel? viewModel =
            viewResult.Model as EmailLinkedToAccountWithRelationshipShutterPageViewModel;
        viewModel!.EmployerAccountLink.Should().Be(EmployerDetailsLink);
    }

    [Test]
    [MoqInlineAutoData(null, 1, "emp name")]
    [MoqInlineAutoData("", 1, "emp name")]
    [MoqInlineAutoData("test@test.com", null, "emp name")]
    [MoqInlineAutoData("Test@test.com", 1, null)]
    public void ShutterPage_SessionDetailsIncomplete_RedirectsToAddEmployerStart(
        string email,
        int? accountLegalEntityId,
        string employerName,
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = employerName
        });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = sut.EmailLinkedToAccountWithRelationshipShutterPage(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void ShutterPage_SessionNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = sut.EmailLinkedToAccountWithRelationshipShutterPage(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
