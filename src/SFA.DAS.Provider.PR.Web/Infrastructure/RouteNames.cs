﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class RouteNames
{
    public const string Home = nameof(Home);
    public const string Employers = nameof(Employers);
    public const string EmployerDetails = nameof(EmployerDetails);
    public const string EmployerDetailsByRequestId = nameof(EmployerDetailsByRequestId);
    public const string RequestPermissionsConfirmation = nameof(RequestPermissionsConfirmation);
    public const string ProviderSignOut = "provider-signout";
    public const string AddEmployerStart = nameof(AddEmployerStart);
    public const string AddEmployerSearchByEmail = nameof(AddEmployerSearchByEmail);
    public const string AddEmployerMultipleAccounts = nameof(AddEmployerMultipleAccounts);
    public const string OneAccountNoRelationship = nameof(OneAccountNoRelationship);
    public const string AddPermissionsAndEmployer = nameof(AddPermissionsAndEmployer);
    public const string AddEmployerConfirmation = nameof(AddEmployerConfirmation);
    public const string AddEmployerSearchByPaye = nameof(AddEmployerSearchByPaye);
    public const string AddEmployerPayeAornNotCorrect = nameof(AddEmployerPayeAornNotCorrect);
    public const string AddEmployerInvitationAlreadySent = nameof(AddEmployerInvitationAlreadySent);
    public const string AddEmployerContactDetails = nameof(AddEmployerContactDetails);
    public const string CheckEmployerDetails = nameof(CheckEmployerDetails);
    public const string ChangePermissions = nameof(ChangePermissions);
    public const string InvitationSent = nameof(InvitationSent);
    public const string PayeAornMatchedEmailNotLinkedNoRelationshipLink = nameof(PayeAornMatchedEmailNotLinkedNoRelationshipLink);
    public const string PayeAornMatchedEmailNotLinkedHasRelationshipLink =
        nameof(PayeAornMatchedEmailNotLinkedHasRelationshipLink);
    public const string EmailLinkedToAccountWithRelationship = nameof(EmailLinkedToAccountWithRelationship);
    public const string EmailSearchInviteAlreadySent = nameof(EmailSearchInviteAlreadySent);
    public const string RequestPermissions = nameof(RequestPermissions);
}
