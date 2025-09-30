using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;

namespace IsekaiFantasyBE.Services.Utils;

public static class Credentials
{
    private static void ValidateEmptyCredentials(UserDTO user)
    {
        if ((string.IsNullOrEmpty(user.Username) && string.IsNullOrEmpty(user.Email))
            || user.Password is null
        )
        {
            throw new ArgumentException(ApiMessages.EmptyCredentials);
        }
    }

    public static void Validate(UserDTO userDto)
    {
        ValidateEmptyCredentials(userDto);
        PasswordService.Validate(userDto.Password);
        if (userDto.Email is null) { return; }
        EmailValidationService.ValidateEmail(userDto.Email);
    }
    
    public static Guid GenerateValidationToken()
    {
        return Guid.NewGuid();
    }
}