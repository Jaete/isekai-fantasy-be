using IsekaiFantasyBE.Models.DTO;

namespace IsekaiFantasyBE.Services.Utils;

public class Credentials
{
    public static bool Validate(UserDTO user)
    {
        if (user.Username is null && user.Email is null || user.Password is null)
        {
            return false;
        }

        return true;
    }
    public static Guid GenerateEmailValidationToken()
    {
        return new Guid();
    }
}