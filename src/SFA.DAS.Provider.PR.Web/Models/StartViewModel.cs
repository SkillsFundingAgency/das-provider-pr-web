namespace SFA.DAS.Provider.PR.Web.Models;

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
