using FluentAssertions;
using SFA.DAS.Provider.PR.Web.Extensions;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Extensions;

public class ObjectExtensionsTests
{
    [Test]
    public void SerializeToDictionary_SerializesSimpleTypes()
    {
        Employee employee = new(101, new("Employee name", null, null), DateTime.Today);

        var result = employee.SerializeToDictionary();

        result.Count.Should().Be(2);
        result.Keys.Should().Contain(nameof(Employee.Id));
        result.Keys.Should().Contain(nameof(Employee.JoiningDate));
        result.Keys.Should().NotContain(nameof(Employee.PersonDetails));
        result.Keys.Should().NotContain(nameof(Employee.PersonDetails.Name));
    }

    [Test]
    public void SerializeToDictionary_SerializesPropertiesWithValue()
    {
        Person employee = new("Employee name", null, string.Empty);

        var result = employee.SerializeToDictionary();

        result.Count.Should().Be(1);
        result.Keys.Should().Contain(nameof(Person.Name));
        result.Keys.Should().NotContain(nameof(Person.Address));
    }

    [Test]
    public void SerializeToDictionary_ReturnsEmptyDictionary()
    {
        Person? person = null;
        person.SerializeToDictionary().Should().BeEmpty();
    }

    private record Employee(int Id, Person PersonDetails, DateTime JoiningDate);
    private record Person(string Name, string? Address, string? PostCode);
}
