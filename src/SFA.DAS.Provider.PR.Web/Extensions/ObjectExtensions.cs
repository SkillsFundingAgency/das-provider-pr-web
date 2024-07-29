using System.Reflection;

namespace SFA.DAS.Provider.PR.Web.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, string> SerializeToDictionary(this object obj)
    {
        var dictionary = new Dictionary<string, string>();

        if (obj == null) return dictionary;

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties.Where(p => p.PropertyType.IsSimpleType()))
        {
            var value = property.GetValue(obj);

            var render = value switch { string x => !string.IsNullOrEmpty(x), _ => value != null };

            if (render)
            {
                dictionary[property.Name] = value!.ToString()!.Trim();
            }
        }

        return dictionary;
    }
}
