using AutoFixture.NUnit3;
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
        Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
        )
    {
        ContactDetailsSubmitModel contactDetailsSubmitModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = Email, FirstName = firstName, LastName = lastName });

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.CheckEmployerDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_NamesAreTrimmed(
        Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        ContactDetailsSubmitModel contactDetailsSubmitModel = new()
        {
            FirstName = $" {firstName}",
            LastName = $"{lastName} "
        };

        var sessionModel = new AddEmployerSessionModel { Email = Email };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        sessionModel.FirstName = firstName;
        sessionModel.LastName = lastName;

        sessionServiceMock.Verify(x => x.Set(It.Is<AddEmployerSessionModel>
            (x => x.FirstName == firstName)), Times.Once);
        sessionServiceMock.Verify(x => x.Set(It.Is<AddEmployerSessionModel>
            (x => x.LastName == lastName)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_FirstNameIsNull_ReturnsValidationError(
        [Frozen] Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        ContactDetailsSubmitModel contactDetailsSubmitModel,
        CancellationToken cancellationToken
    )
    {
        contactDetailsSubmitModel.FirstName = null;

        sessionServiceMock.Setup(s => 
            s.Get<AddEmployerSessionModel>()
        ).Returns(
            new AddEmployerSessionModel { Email = Email }
        );

        var validationFailures = new List<ValidationFailure>
        {
            new("FirstName", "FirstName field is invalid") { ErrorCode = "1001" }
        };

        validatorMock
            .Setup(m => m.Validate(It.IsAny<ContactDetailsSubmitModel>()))
            .Returns(new ValidationResult(validationFailures));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        var viewResult = result.As<ViewResult>();
        Assert.That(viewResult, Is.Not.Null);

        Assert.Multiple(() =>
        {
            var emailError = viewResult.ViewData.ModelState["FirstName"]?.Errors.FirstOrDefault();
            Assert.That(emailError, Is.Not.Null, "Expected a validation error for the 'FirstName' field.");
            Assert.That(emailError!.ErrorMessage, Is.EqualTo("FirstName field is invalid"));
        });
    }

    [Test, MoqAutoData]
    public void Post_FirstNameIsEmpty_ReturnsValidationError(
        [Frozen] Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        ContactDetailsSubmitModel contactDetailsSubmitModel,
        CancellationToken cancellationToken
    )
    {
        contactDetailsSubmitModel.FirstName = "  ";

        sessionServiceMock.Setup(s =>
            s.Get<AddEmployerSessionModel>()
        ).Returns(
            new AddEmployerSessionModel { Email = Email }
        );

        var validationFailures = new List<ValidationFailure>
        {
            new("FirstName", "FirstName field is invalid") { ErrorCode = "1001" }
        };

        validatorMock
            .Setup(m => m.Validate(It.IsAny<ContactDetailsSubmitModel>()))
            .Returns(new ValidationResult(validationFailures));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        var viewResult = result.As<ViewResult>();
        Assert.That(viewResult, Is.Not.Null);

        Assert.Multiple(() =>
        {
            var emailError = viewResult.ViewData.ModelState["FirstName"]?.Errors.FirstOrDefault();
            Assert.That(emailError, Is.Not.Null, "Expected a validation error for the 'FirstName' field.");
            Assert.That(emailError!.ErrorMessage, Is.EqualTo("FirstName field is invalid"));
        });
    }

    [Test, MoqAutoData]
    public void Post_LastNameIsNull_ReturnsValidationError(
        [Frozen] Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        ContactDetailsSubmitModel contactDetailsSubmitModel,
        CancellationToken cancellationToken
    )
    {
        contactDetailsSubmitModel.LastName = null;

        sessionServiceMock.Setup(s =>
            s.Get<AddEmployerSessionModel>()
        ).Returns(
            new AddEmployerSessionModel { Email = Email }
        );

        var validationFailures = new List<ValidationFailure>
        {
            new("LastName", "LastName field is invalid") { ErrorCode = "1001" }
        };

        validatorMock
            .Setup(m => m.Validate(It.IsAny<ContactDetailsSubmitModel>()))
            .Returns(new ValidationResult(validationFailures));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        var viewResult = result.As<ViewResult>();
        Assert.That(viewResult, Is.Not.Null);

        Assert.Multiple(() =>
        {
            var emailError = viewResult.ViewData.ModelState["LastName"]?.Errors.FirstOrDefault();
            Assert.That(emailError, Is.Not.Null, "Expected a validation error for the 'LastName' field.");
            Assert.That(emailError!.ErrorMessage, Is.EqualTo("LastName field is invalid"));
        });
    }

    [Test, MoqAutoData]
    public void Post_LastNameIsEmpty_ReturnsValidationError(
        [Frozen] Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        ContactDetailsSubmitModel contactDetailsSubmitModel,
        CancellationToken cancellationToken
    )
    {
        contactDetailsSubmitModel.LastName = "  ";

        sessionServiceMock.Setup(s =>
            s.Get<AddEmployerSessionModel>()
        ).Returns(
            new AddEmployerSessionModel { Email = Email }
        );

        var validationFailures = new List<ValidationFailure>
        {
            new("LastName", "LastName field is invalid") { ErrorCode = "1001" }
        };

        validatorMock
            .Setup(m => m.Validate(It.IsAny<ContactDetailsSubmitModel>()))
            .Returns(new ValidationResult(validationFailures));

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var result = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        var viewResult = result.As<ViewResult>();
        Assert.That(viewResult, Is.Not.Null);

        Assert.Multiple(() =>
        {
            var emailError = viewResult.ViewData.ModelState["LastName"]?.Errors.FirstOrDefault();
            Assert.That(emailError, Is.Not.Null, "Expected a validation error for the 'LastName' field.");
            Assert.That(emailError!.ErrorMessage, Is.EqualTo("LastName field is invalid"));
        });
    }

    [Test, MoqAutoData]
    public void Post_SessionModelNotFound_RedirectedToStart(
        Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
       )
    {
        ContactDetailsSubmitModel contactDetailsSubmitModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Post_RedirectsToStartIfEmailNotSetInSession(
        Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
    )
    {
        ContactDetailsSubmitModel contactDetailsSubmitModel = new()
        {
            FirstName = firstName,
            LastName = lastName
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        validatorMock.Setup(v => v.Validate(It.IsAny<ContactDetailsSubmitModel>())).Returns(new ValidationResult());

        ContactDetailsController sut = new(sessionServiceMock.Object, validatorMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink);

        var actual = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);
        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Post_Invalid_ReturnsExpectedViewModelAndPath(
        Mock<IValidator<ContactDetailsSubmitModel>> validatorMock,
        Mock<ISessionService> sessionServiceMock,
        int ukprn,
        string firstName,
        string lastName,
        CancellationToken cancellationToken
    )
    {
        ContactDetailsSubmitModel contactDetailsSubmitModel = new()
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

        var result = sut.Index(ukprn, contactDetailsSubmitModel, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        ContactDetailsViewModel? viewModel = viewResult.Model as ContactDetailsViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<AddEmployerSessionModel>()), Times.Never);
        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
    }
}
