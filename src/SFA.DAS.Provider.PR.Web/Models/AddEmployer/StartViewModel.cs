namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class StartViewModel
{
    public string ContinueLink { get; }
    public string ViewEmployersAndPermissionsLink { get; }
    public StartViewModel(string continueLink, string viewEmployersAndPermissionsLink)
    {
        ContinueLink = continueLink;
        ViewEmployersAndPermissionsLink = viewEmployersAndPermissionsLink;
    }
}
