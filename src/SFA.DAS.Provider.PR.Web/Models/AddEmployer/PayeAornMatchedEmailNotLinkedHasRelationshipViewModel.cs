namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class PayeAornMatchedEmailNotLinkedHasRelationshipViewModel
{
    public required string EmployerName { get; set; }
    public required string PayeReference { get; set; }
    public required string Aorn { get; set; }
    public required string Email { get; set; }
    public required string EmployerAccountLink { get; set; }
};