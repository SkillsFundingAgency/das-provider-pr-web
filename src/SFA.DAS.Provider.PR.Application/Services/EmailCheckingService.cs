using System.Net;

namespace SFA.DAS.Provider.PR.Application.Services;
public static class EmailCheckingService
{
    public static bool IsValidDomain(string? email)
    {
        if (email == null)
        {
            return false;
        }

        var domain = email.Contains('@')
            ? email.Split('@')[1]
            : email;

        if (string.IsNullOrEmpty(domain))
        {
            return false;
        }

        try
        {
            var hostEntry = Dns.GetHostEntry(domain);

            if (hostEntry.AddressList.Length > 0)
            {
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }
}
