using System.Security.Cryptography;
using System.Text;
using IsekaiFantasyBE.Models.Response;

namespace IsekaiFantasyBE.Services.Utils;

public static class PasswordService
{
    public static void Validate(string? password)
    {
        if (password is null || password.Length < 8)
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

        if (password.All(char.IsLetterOrDigit))
        {
            throw new ArgumentException(ApiMessages.PasswordInvalidSpecial);
        }
    }

    public static byte[] Encrypt(string password)
    {
        if (password is null)
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null.");
        }
        
        return SHA256
            .Create()
            .ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    
    public static bool Verify(string password, byte[] hash)
    {
        var bytes = Encrypt(password);
        return bytes.SequenceEqual(hash);
    }
}