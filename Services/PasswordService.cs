using System.Security.Cryptography;
using System.Text;
using IsekaiFantasyBE.Models.Response;

namespace IsekaiFantasyBE.Services;

public static class PasswordService
{
    public static void Validate(string password)
    {
        if (password.Length < 8)
        {
            throw new ArgumentException(ApiMessages.PasswordInvalidLength);
        }

        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException(ApiMessages.PasswordInvalidUpper);
        }

        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException(ApiMessages.PasswordInvalidLower);
        }
        
        if(!password.Any(char.IsDigit))
        {
            throw new ArgumentException(ApiMessages.PasswordInvalidDigit);
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new ArgumentException(ApiMessages.PasswordInvalidSpecial);
        }
    }

    public static byte[] Encrypt(string password)
    {
        using var sha256Hash = SHA256.Create();

        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        return bytes;
    }
    
    public static bool Verify(string password, byte[] hash)
    {
        using var sha256Hash = SHA256.Create();

        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        return bytes.SequenceEqual(hash);
    }
}