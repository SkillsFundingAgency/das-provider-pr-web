﻿namespace SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
public class GetRelationshipByEmailResponse
{
    public bool HasActiveRequest { get; set; }
    public Guid? RequestId { get; set; }
    public bool? HasUserAccount { get; set; }
    public bool? HasOneEmployerAccount { get; set; }
    public long? AccountId { get; set; }
    public bool? HasOneLegalEntity { get; set; }
    public string? AccountLegalEntityPublicHashedId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string? AccountLegalEntityName { get; set; }
    public string? Paye { get; set; }
    public bool? HasRelationship { get; set; }
    public List<Operation> Operations { get; set; } = new List<Operation>();
}