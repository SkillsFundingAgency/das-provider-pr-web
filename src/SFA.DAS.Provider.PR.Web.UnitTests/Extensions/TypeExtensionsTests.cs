using FluentAssertions;
using SFA.DAS.Provider.PR.Web.Extensions;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Extensions;

public class TypeExtensionsTests
{
    [TestCase(typeof(string), true)]
    [TestCase(typeof(DateTime), true)]
    [TestCase(typeof(decimal), true)]
    [TestCase(typeof(int), true)]
    [TestCase(typeof(long), true)]
    [TestCase(typeof(Gender), true)]
    [TestCase(typeof(Person), false)]
    [TestCase(typeof(IPerson), false)]
    public void IsSimpleType_ReturnsResult(Type type, bool expected)
    {
        type.IsSimpleType().Should().Be(expected);
    }

    private enum Gender { Male, Female }
    private class Person : IPerson { }
    private interface IPerson;
}
