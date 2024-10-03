using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.CheckDetails;
public class CheckDetailsControllerPostTests
{
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    private static readonly string Email = "test@account.com";

    [Test, MoqAutoData]
    public async Task Post_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn,
        string firstName,
        string lastName,
        AddEmployerSessionModel addEmployerSessionModel,
        CancellationToken cancellationToken
    )
    {
        addEmployerSessionModel.Email = Email;
        addEmployerSessionModel.FirstName = firstName;
        addEmployerSessionModel.LastName = lastName;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        sut.AddDefaultContext();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = await sut.Index(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.InvitationSent);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Never);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Post_SessionModelNotFound_RedirectedToStart(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn,
        CancellationToken cancellationToken
      )
    {

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var actual = await sut.Index(ukprn, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public async Task Post_RedirectsToStartIfEmailNotSetInSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var actual = await sut.Index(ukprn, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }
}
