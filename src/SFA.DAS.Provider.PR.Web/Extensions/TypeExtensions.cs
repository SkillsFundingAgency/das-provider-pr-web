namespace SFA.DAS.Provider.PR.Web.Extensions;

public static class TypeExtensions
{
    public static bool IsSimpleType(this Type type)
    {
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(DateTime) ||
               type == typeof(decimal);
    }

}
