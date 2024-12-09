﻿using DnsClient;
using DnsClient.Protocol;

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

        var lookup = new LookupClient();

        var results = lookup.Query(domain, QueryType.MX).Answers;

        return results.Any(x => x.RecordType == ResourceRecordType.MX);
    }
}
