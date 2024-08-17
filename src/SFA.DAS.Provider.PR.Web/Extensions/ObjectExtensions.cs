using System.Reflection;

namespace SFA.DAS.Provider.PR.Web.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, object> ConvertToDictionary(this object obj)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        PropertyInfo[] properties = obj.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
            {
                var value = property.GetValue(obj);
                if (value != null) dict[property.Name] = value;
            }
        }

        return dict;
    }
}
