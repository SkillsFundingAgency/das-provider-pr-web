using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Services;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Services;
public class EmployerDetailsMappingServiceMapLastActionTextByRequestIdTests
{
    [Test]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, new Operation[0],
        EmployerDetailsMappingService.PendingAddTrainingProviderAndPermissionsRequestText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeCreateAccount, new Operation[0],
        EmployerDetailsMappingService.PendingCreateAccountInvitationText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, new Operation[0],
        EmployerDetailsMappingService.PendingAddTrainingProviderAndPermissionsRequestText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, new[] { Operation.CreateCohort },
        EmployerDetailsMappingService.UpdatePermissionRequestSentText)]
    [InlineAutoData("", new Operation[0], "")]
    public void MapTextByRequestId_LastActionTextIsSet(string requestType, Operation[] operations,
        string expected, GetRequestsByRequestIdResponse response)
    {
        response.Operations = operations;
        response.RequestType = requestType;

        var result = EmployerDetailsMappingService.MapLastActionTextByRequestId(response);

        Assert.That(result, Is.EqualTo(expected));
    }
}
