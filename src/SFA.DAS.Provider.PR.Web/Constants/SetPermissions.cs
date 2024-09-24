using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Constants;

[ExcludeFromCodeCoverage]
public static class SetPermissions
{
    public static class AddRecords
    {
        public const string Yes = "Yes";
        public const string No = "No";
    }

    public static class RecruitApprentices
    {
        public const string Yes = "Yes";
        public const string YesWithReview = "YesWithReview";
        public const string No = "No";
    }
}

public static class SetPermissionsText
{
    public const string NoPermissionText = "No";
    public const string RecruitmentPermissionText = "Yes";
    public const string CohortsPermissionText = "Yes, employer will review records";
    public const string RecruitmentWithReviewPermissionText = "Yes, employer will review adverts";
}
