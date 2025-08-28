using System.Security.Cryptography;
using System.Text;

namespace IsekaiFantasyBE.Services.Utils;

public static class Encryption
{
    public static byte[] Encrypt(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null.");
        }

        using var sha256Hash = SHA256.Create();

        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        return bytes;
    }

    public static bool Verify(string password, byte[] hash)
    {
        var bytes = Encrypt(password);
        return bytes.SequenceEqual(hash);
    }

    public static bool Compare(byte[] check, byte[] reference)
    {
        return reference.SequenceEqual(check);
    }

}