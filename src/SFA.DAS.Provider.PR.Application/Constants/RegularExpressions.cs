namespace SFA.DAS.Provider.PR.Application.Constants;
public static class RegularExpressions
{
    public const string EmailRegex = @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z0-9_](-?[a-zA-Z0-9_])*(\.[a-zA-Z0-9](-?[a-zA-Z0-9])*)+$";
    public const string PayeRegex = @"^[0-9]{3}/?[A-Za-z0-9]{1,10}$";
    public const string AornRegex = @"^[A-Za-z0-9]{13}$";
}