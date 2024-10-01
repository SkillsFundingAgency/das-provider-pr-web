﻿namespace SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
public class GetProviderRelationshipResponse
{
    public long AccountLegalEntityId { get; set; }

    public string AccountLegalEntityPublicHashedId { get; set; } = null!;

    public string AccountLegalEntityName { get; set; } = null!;

    public long AccountId { get; set; }

    public long Ukprn { get; set; }

    public string ProviderName { get; set; } = null!;

    public Operation[] Operations { get; set; } = [];

    public PermissionAction? LastAction { get; set; }

    public DateTime? LastActionTime { get; set; }

    public string? LastRequestType { get; set; }

    public DateTime? LastRequestTime { get; set; }

    public RequestStatus? LastRequestStatus { get; set; }

    public Operation[]? LastRequestOperations { get; set; } = [];
}

public enum PermissionAction : short
{
    ApprovalsRelationship,
    RecruitRelationship,
    PermissionCreated,
    PermissionUpdated,
    PermissionDeleted
}

public enum RequestStatus : short
{
    New,
    Sent,
    Accepted,
    Declined,
    Expired,
    Deleted
}
