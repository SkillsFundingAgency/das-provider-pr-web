using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByPaye;
public class PayeAornMatchedEmailNotLinkedShutterPagePostTests
{
    private static readonly string StartLink = Guid.NewGuid().ToString();
    private static readonly string NextPageLink = Guid.NewGuid().ToString();
    private const string Email = "test@test.com";


    [Test, MoqAutoData]
    public void Post_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        AddEmployerSessionModel sessionModel,
        int ukprn,
        AddPermissionsAndEmployerViewModel addPermissionsAndEmployerViewModel
    )
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        validatorMock.Setup(v => v.Validate(It.IsAny<SearchByEmailSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, NextPageLink);

        var result = sut.PostPayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddPermissionsAndEmployer);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test]
    [MoqInlineAutoData(null, null, 2)]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, null, 1)]
    [MoqInlineAutoData(null, SetPermissions.RecruitApprentices.Yes, 1)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes, 0)]
    public void Get_SetsSessionModelIfUpdatingPermissions(
        string? permissionToAddCohorts,
        string? permissionsToRecruit,
        int numberOfSessionSets,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        AddEmployerSessionModel addEmployerSessionModel)
    {
        var email = "test@test.com";
        addEmployerSessionModel.Email = email;
        addEmployerSessionModel.IsCheckDetailsVisited = true;

        addEmployerSessionModel.PermissionToAddCohorts = permissionToAddCohorts;
        addEmployerSessionModel.PermissionToRecruit = permissionsToRecruit;

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, StartLink);

        sut.PostPayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        sessionServiceMock.Verify(x => x.Set(It.IsAny<AddEmployerSessionModel>()), Times.Exactly(numberOfSessionSets));
    }

    [Test, MoqAutoData]
    public void Get_SessionModelNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, StartLink);

        var result = sut.PostPayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void Get_SessionModelInvalid_RedirectsToAddEmployerStart(
        string email,
        long? accountLegalEntityId,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email!, Paye = null, AccountLegalEntityId = accountLegalEntityId, AccountLegalEntityName = "name" });

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, StartLink);

        var result = sut.PostPayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
