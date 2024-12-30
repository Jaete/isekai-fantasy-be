using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Repository;
using OneOf;

namespace IsekaiFantasyBE.Services;

public class UserService
{
    private UserRepository _userRepo;

    public UserService(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }
    
    private static ResponseModel CreateSingleUserResponse(User? user)
    {
        if (user is null)
        {
            return ResponseModel.Write(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
        }

        return ResponseModel.Write(user, ApiMessages.UserRetrieved, StatusCodes.Status200OK);
    }
    
    private static ResponseModel CreateEmailInvalidResponse(string email)
    {
        return ResponseModel.Write(null!, ApiMessages.EmailInvalid, StatusCodes.Status400BadRequest)!;
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
        return CreateSingleUserResponse(user);
    }

    public async Task<ResponseModel> GetUserByEmail(string email)
    {
        if (!EmailValidationService.IsValidEmail(email))
        {
            return CreateEmailInvalidResponse(email);
        }
        
        var user = await _userRepo.GetUserByEmail(email);
        return CreateSingleUserResponse(user);
    }

    public async Task<ResponseModel> GetUserByUsername(string username, HttpContext context)
    {
        var user = await _userRepo.GetUserByUsername(username);
        return CreateSingleUserResponse(user);
    }

    public async Task<ResponseModel> RegisterNewUser(UserDTO userDto)
    {
        try
        {
            ValidateCredentials(userDto);
            
            var user = new User
            {
                Username = userDto.Username!,
                Email = userDto.Email!,
                Password = PasswordService.Encrypt(userDto.Password!),
            };
            await _userRepo.RegisterNewUser(user);
            return ResponseModel.Write(user, ApiMessages.UserCreated, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<ResponseModel> LoginUser(UserDTO userDto)
    {
        try
        {
            ValidateCredentials(userDto);
            var user = await _userRepo.GetUserByUsername(userDto.Username!);
            if (user is null)
            {
                return ResponseModel.Write(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
            }
            
            if(!PasswordService.Verify(userDto.Password!, user.Password))
            {
                return ResponseModel.Write(null!, ApiMessages.WrongPassword, StatusCodes.Status400BadRequest);
            }

            var token = JwtService.GenerateJwtToken(user);
            return ResponseModel.Write(token, ApiMessages.LoginSuccess, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ResponseModel> UpdateProperties(UserPropertiesDTO properties, HttpContext context)
    {
        try
        {
            var userId = JwtService.GetAuthenticatedUserId(context);
            var user = await _userRepo.GetUserById(userId);
            
            if (user is null)
            {
                return ResponseModel.Write(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            var newUserProperties = new UserProperties
            {
                User = user,
                LastActivity = DateTime.Now,
                Bio = properties.Bio,
                Photo = properties.Photo,
            };

            await _userRepo.UpdateUserProperties(newUserProperties);

            return ResponseModel.Write(user, ApiMessages.UserUpdated, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
