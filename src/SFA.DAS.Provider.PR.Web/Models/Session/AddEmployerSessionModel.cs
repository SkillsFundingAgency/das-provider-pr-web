namespace SFA.DAS.Provider.PR.Web.Models.Session;

public class AddEmployerSessionModel
{
    public string Email { get; }

    public AddEmployerSessionModel(string email)
    {
        Email = email;
    }
}
