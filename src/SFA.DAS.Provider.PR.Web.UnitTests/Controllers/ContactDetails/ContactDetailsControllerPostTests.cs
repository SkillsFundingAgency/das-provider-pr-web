using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.ContactDetails;
public class ContactDetailsControllerPostTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    private static readonly string Email = "test@account.com";

    [Test, MoqAutoData]
    public void Post_ReturnsExpectedViewModelAndPath(
        Mock<IValidator<ContactDetailsSubmitViewModel>> validatorMock,
     Mock<ISessionService> sessionServiceMock,
     int ukprn,
     string firstName,
     string lastName,
     CancellationToken cancellationToken
     )
    {
        ContactDetailsSubmitViewModel contactDetailsSubmitViewModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = Email, FirstName = firstName, LastName = lastName });

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitViewModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitViewModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerContactDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_SessionModelNotFound_RedirectedToStart(
        Mock<IValidator<ContactDetailsSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
       )
    {
        ContactDetailsSubmitViewModel contactDetailsSubmitViewModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitViewModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = sut.Index(ukprn, contactDetailsSubmitViewModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Post_RedirectsToStartIfEmailNotSetInSession(
        Mock<IValidator<ContactDetailsSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
    )
    {
        ContactDetailsSubmitViewModel contactDetailsSubmitViewModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitViewModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = sut.Index(ukprn, contactDetailsSubmitViewModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Post_Invalid_ReturnsExpectedViewModelAndPath(
        Mock<IValidator<ContactDetailsSubmitViewModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
    )
    {
        ContactDetailsSubmitViewModel contactDetailsSubmitViewModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        validatorMock.Setup(m => m.Validate(It.IsAny<ContactDetailsViewModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = Email });

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitViewModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        ContactDetailsViewModel? viewModel = viewResult.Model as ContactDetailsViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Never);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }
}
