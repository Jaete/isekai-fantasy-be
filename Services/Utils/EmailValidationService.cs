using System.Text.RegularExpressions;
using IsekaiFantasyBE.Models.Response;

namespace IsekaiFantasyBE.Services.Utils;

public class EmailValidationService
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool ValidateEmail(string? email, bool throwError = true)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
        {
            return throwError ? throw new ArgumentException(ApiMessages.EmailInvalid) : false;
        }

        return true;
    }
}