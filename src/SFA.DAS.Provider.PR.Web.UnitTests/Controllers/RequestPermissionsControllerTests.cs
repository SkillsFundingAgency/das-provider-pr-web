using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Application.Constants;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;
public class RequestPermissionsControllerTests
{
    private Mock<IOuterApiClient> _outerApiClientMock;
    private Mock<IEncodingService> _encodingServiceMock;
    private Mock<IValidator<RequestPermissionsSubmitModel>> _validatorMock;
    private RequestPermissionsController sut;

    [SetUp]
    public void Setup()
    {
        _outerApiClientMock = new Mock<IOuterApiClient>();
        _outerApiClientMock.Setup(x =>
            x.GetProviderRelationship(
                It.IsAny<long>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            new GetProviderRelationshipResponse() { 
                AccountLegalEntityName = "AccountLegalEntityName" ,
                Operations = new[] { Operation.Recruitment, Operation.CreateCohort }
            }
        );

        _encodingServiceMock = new Mock<IEncodingService>();
        _validatorMock = new Mock<IValidator<RequestPermissionsSubmitModel>>();

        sut = new RequestPermissionsController(
            _outerApiClientMock.Object,
            _encodingServiceMock.Object,
            _validatorMock.Object
        );

        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Employers, "employers-url");
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetails,"employer-details-url");

        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
    }

    [Test]
    public async Task RequestPermissionsController_Index_RedirectsIfRequestIdExists()
    {
        sut.TempData[TempDataKeys.PermissionsRequestId] = "existingRequestId";

        var result = await sut.Index(12345, "accountLegalEntityId", CancellationToken.None);

        var redirectResult = (RedirectToRouteResult)result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<RedirectToRouteResult>());
            Assert.That(redirectResult.RouteName, Is.EqualTo(RouteNames.Employers));
            Assert.That((bool)redirectResult?.RouteValues!["HasPendingRequest"]!, Is.True);
        });
    }

    [Test]
    public async Task RequestPermissionsController_Index_ReturnsViewWithViewModel()
    {
        sut.TempData.Remove(TempDataKeys.PermissionsRequestId);
        var viewModel = new RequestPermissionsViewModel();

        _encodingServiceMock.Setup(x => 
            x.Decode(
                It.IsAny<string>(), 
                EncodingType.PublicAccountLegalEntityId
            )
        ).Returns(123);

        var result = await sut.Index(12345, "accountLegalEntityId", CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<RequestPermissionsViewModel>());

            var viewModel = ((RequestPermissionsViewModel)viewResult.Model!)!;
            Assert.That(viewModel.AccountLegalEntityName, Is.EqualTo("ACCOUNTLEGALENTITYNAME"));
            Assert.That(viewModel.ExistingPermissionToAddCohorts, Is.EqualTo(nameof(SetPermissions.AddRecords.Yes)));
            Assert.That(viewModel.ExistingPermissionToRecruit, Is.EqualTo(nameof(SetPermissions.RecruitApprentices.Yes)));
            Assert.That(viewModel.PermissionToAddCohorts, Is.EqualTo(nameof(SetPermissions.AddRecords.Yes)));
            Assert.That(viewModel.PermissionToRecruit, Is.EqualTo(nameof(SetPermissions.RecruitApprentices.Yes)));
        });
    }

    [Test]
    public async Task RequestPermissionsController_Index_ModelInvalid_ReturnsView()
    {
        var submitModel = new RequestPermissionsSubmitModel();

        _validatorMock.Setup(x => 
            x.Validate(
                It.IsAny<RequestPermissionsSubmitModel>()
            )
        ).Returns(
            new ValidationResult(
                new List<ValidationFailure> { 
                    new ValidationFailure("field", "error")
                }
            )
        );

        _encodingServiceMock.Setup(x => 
            x.Decode(
                It.IsAny<string>(), 
                EncodingType.PublicAccountLegalEntityId
            )
        ).Returns(123);

        var result = await sut.Index(12345, "accountLegalEntityId", submitModel, CancellationToken.None);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<RequestPermissionsViewModel>());
        });
    }

    [Test]
    public async Task RequestPermissionsController_Index_ModelValid_RedirectsToConfirmationOnSuccess()
    { 
        var submitModel = new RequestPermissionsSubmitModel();

        _validatorMock.Setup(x => 
            x.Validate(
                It.IsAny<RequestPermissionsSubmitModel>()
            )
        ).Returns(new ValidationResult());

        _encodingServiceMock.Setup(x => 
            x.Decode(
                It.IsAny<string>(), 
                EncodingType.PublicAccountLegalEntityId
            )
        ).Returns(123);

        _outerApiClientMock.Setup(x => 
            x.GetProviderRelationship(
                It.IsAny<long>(), 
                It.IsAny<long>(), 
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            new GetProviderRelationshipResponse { 
                AccountLegalEntityName = "AccountLegalEntityName", 
                AccountId = 456 
            }
        );

        var requestId = Guid.NewGuid();
        _outerApiClientMock.Setup(x => 
            x.CreatePermissions(
                It.IsAny<CreatePermissionRequestCommand>(), 
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new CreatePermissionRequestResponse(requestId));

        var result = await sut.Index(12345, "accountLegalEntityId", submitModel, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(sut.TempData.ContainsKey(TempDataKeys.AccountLegalEntityName), Is.True);
            Assert.That(sut.TempData[TempDataKeys.AccountLegalEntityName], Is.EqualTo("ACCOUNTLEGALENTITYNAME"));
            Assert.That(sut.TempData.ContainsKey(TempDataKeys.PermissionsRequestId), Is.True);
            Assert.That(sut.TempData[TempDataKeys.PermissionsRequestId], Is.EqualTo(requestId));
            Assert.That(result, Is.InstanceOf<RedirectToRouteResult>());
            var redirectResult = (RedirectToRouteResult)result;
            Assert.That(redirectResult.RouteName, Is.EqualTo(RouteNames.RequestPermissionsConfirmation));
        });
    }
}
