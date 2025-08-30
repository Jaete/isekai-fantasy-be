using System.Text.RegularExpressions;
using IsekaiFantasyBE.Models.Response;

namespace IsekaiFantasyBE.Services.Utils;

public class EmailValidationService
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static void IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
        {
            throw new ArgumentException(ApiMessages.EmailInvalid);
        }
    }
}