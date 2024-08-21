using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Services;

public static class OperationsMappingService
{
    public static List<Operation> MapDescriptionsToOperations(PermissionDescriptionsViewModel permissionDescriptionsViewModel)
    {
        var operations = new List<Operation>();

        if (permissionDescriptionsViewModel.PermissionToAddCohorts == SetPermissions.AddRecords.Yes)
        {
            operations.Add(Operation.CreateCohort);
        }

        if (permissionDescriptionsViewModel.PermissionToRecruit == SetPermissions.RecruitApprentices.Yes)
        {
            operations.Add(Operation.Recruitment);
        }
        else if (permissionDescriptionsViewModel.PermissionToRecruit == SetPermissions.RecruitApprentices.YesWithReview)
        {
            operations.Add(Operation.RecruitmentRequiresReview);
        }

        return operations;
    }
}

