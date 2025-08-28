using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Response.Entities;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Repository;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.OpenApi.Extensions;

namespace IsekaiFantasyBE.Services;

public class UserService
{
    private UserRepository _userRepo;

    public UserService(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    private static bool ValidateEmptyCredentials(UserDTO user)
    {
        if (user.Username is null || user.Password is null)
        {
            return false;
        }

        return true;
    }

    private static void ValidateCredentials(UserDTO userDto)
    {
        if (!ValidateEmptyCredentials(userDto))
        {
            throw new ArgumentException(ApiMessages.EmptyCredentials);
        }
            
        if (!EmailValidationService.IsValidEmail(userDto.Email))
        {
            throw new ArgumentException(ApiMessages.EmailInvalid);
        }
            
        PasswordService.Validate(userDto.Password);
    }

    public async Task<ResponseModel> GetUserById(Guid id)
    {
        var user = await _userRepo.GetUserById(id);
        
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound)
            : ResponseService.Ok(
            new UserResponse(
                user.Id,
                user.Username,
                user.Properties,
                user.CreatedAt,
                user.UpdatedAt,
                user.LastLogin
            ),
            ApiMessages.UserRetrieved
        );
    }

    public async Task<ResponseModel> GetUserByEmail(string email)
    {
        if (!EmailValidationService.IsValidEmail(email))
        {
            return ResponseService.BadRequest(ApiMessages.EmailInvalid);
        }
        
        var user = await _userRepo.GetUserByEmail(email);
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound) 
            : ResponseService.Ok(
                new UserResponse(
                    user.Id,
                    user.Username,
                    user.Properties,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.LastLogin
                ),
                ApiMessages.UserRetrieved
            );
    }

    public async Task<ResponseModel> GetUserByUsername(string username, HttpContext context)
    {
        var user = await _userRepo.GetUserByUsername(username);
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound) 
            : ResponseService.Ok(
                new UserResponse(
                    user.Id,
                    user.Username,
                    user.Properties,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.LastLogin
                ),
                ApiMessages.UserRetrieved
            );
    }

    public async Task<ResponseModel> RegisterNewUser(UserDTO userDto)
    {
        ValidateCredentials(userDto);
        
        var user = new User
        {
            Username = userDto.Username!,
            Email = userDto.Email!,
            Password = PasswordService.Encrypt(userDto.Password!),
        };
        await _userRepo.RegisterNewUser(user);
        
        return ResponseService.Created(
            new UserResponse(user.Id, user.Username),
            ApiMessages.UserCreated
        );
    }
    
    public async Task<ResponseModel> LoginUser(UserDTO userDto)
    {
        ValidateCredentials(userDto);
        var user = await _userRepo.GetUserByUsername(userDto.Username!);
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }
        
        if(!PasswordService.Verify(userDto.Password!, user.Password))
        {
            return ResponseService.BadRequest(ApiMessages.WrongPassword);
        }

        var token = JwtService.GenerateJwtToken(user);
        return ResponseService.Ok(token, ApiMessages.LoginSuccess);
    }

    public async Task<ResponseModel> UpdateProperties(UserPropertiesDTO properties, HttpContext context)
    {
        var userId = JwtService.GetAuthenticatedUserId(context);
        var user = await _userRepo.GetUserById(userId);

        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }
        
        await _userRepo.UpdateUserProperties(
            new UserProperties
            {
                User = user,
                LastActivity = DateTime.Now,
                Bio = properties.Bio,
                Photo = properties.Photo,
            }    
        );

        return ResponseService.Ok(
            new UserResponse(user.Id, user.Username), ApiMessages.UserUpdated
        );
    }
}
