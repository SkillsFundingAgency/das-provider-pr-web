﻿using FluentAssertions;
using SFA.DAS.Provider.PR.Application.Services;

namespace SFA.DAS.Provider.PR.Application.UnitTests.Services;
public class EmailCheckingServiceTests
{
    [TestCase("xxx", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [TestCase("aaaa@", false)]
    [TestCase("@aaaa", false)]
    [TestCase("aaaa@NonExistentDomain50c2413d-e8e4-4330-9859-222567ad0f64.co.uk", false)]
    [TestCase("aaaa@google.com", true)]
    public void Email_IsValidDomain_ReturnedExpected(string? email, bool isValid)
    {
        var isEmailValid = EmailCheckingService.IsValidDomain(email);
        isEmailValid.Should().Be(isValid);
    }
}