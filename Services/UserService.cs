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
    
    private static Response<User?> CreateSingleUserResponse(User? user)
    {
        if (user is null)
        {
            return Response<User?>.FailUser(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
        }

        return Response<User?>.SuccessUser(user, ApiMessages.UserRetrieved, StatusCodes.Status200OK);
    }
    
    private static Response<User?> CreateEmailInvalidResponse(string email)
    {
        return Response<User>.FailUser(null!, ApiMessages.EmailInvalid, StatusCodes.Status400BadRequest)!;
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

    public async Task<Response<User?>> GetUserById(Guid id, HttpContext context)
    {
        var response = JwtService.RequireAuthentication(context);
        if(response.StatusCode == StatusCodes.Status401Unauthorized){
            return response;
        }
        var user = await _userRepo.GetUserById(id);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<User?>> GetUserByEmail(string email, HttpContext context)
    {
        var response = JwtService.RequireAuthentication(context);
        if(response.StatusCode == StatusCodes.Status401Unauthorized){
            return response;
        }
        
        if (!EmailValidationService.IsValidEmail(email))
        {
            return CreateEmailInvalidResponse(email);
        }
        
        var user = await _userRepo.GetUserByEmail(email);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<User?>> GetUserByUsername(string username, HttpContext context)
    {
        var response = JwtService.RequireAuthentication(context);
        if(response.StatusCode == StatusCodes.Status401Unauthorized){
            return response;
        }
        
        var user = await _userRepo.GetUserByUsername(username);
        response = CreateSingleUserResponse(user);
        return response;
    }

    public async Task<Response<User?>> RegisterNewUser(UserDTO userDto)
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
            return Response<User?>.SuccessUser(user, ApiMessages.UserCreated, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return Response<User?>.FailWithException(null!, ex.Message, ExceptionService.GetStatusCode(ex), ex.StackTrace!);
        }
    }
    
    public async Task<Response<string?>> LoginUser(UserDTO userDto)
    {
        try
        {
            ValidateCredentials(userDto);
            var user = await _userRepo.GetUserByUsername(userDto.Username!);
            if (user is null)
            {
                return Response<string?>.FailString(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
            }
            
            if(!PasswordService.Verify(userDto.Password!, user.Password))
            {
                return Response<string?>.FailString(null!, ApiMessages.WrongPassword, StatusCodes.Status400BadRequest);
            }

            var token = JwtService.GenerateJwtToken(user);
            return Response<string>.SuccessString(token, ApiMessages.LoginSuccess, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return Response<string?>.FailStringWithException("", e.Message, ExceptionService.GetStatusCode(e), e.StackTrace!);
        }
    }

    public async Task<Response<User?>> UpdateProperties(UserPropertiesDTO properties, HttpContext context)
    {
        var response = JwtService.RequireAuthentication(context);
        if(response.StatusCode == StatusCodes.Status401Unauthorized){
            return response;
        }

        try
        {
            var userId = JwtService.GetAuthenticatedUserId(context);
            var user = await _userRepo.GetUserById(userId);
            
            if (user is null)
            {
                return Response<User?>.FailUser(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            var newUserProperties = new UserProperties();

            newUserProperties.User = user;
            newUserProperties.LastActivity = DateTime.Now;
            newUserProperties.Bio = properties.Bio;
            newUserProperties.Photo = properties.Photo;
            
            await _userRepo.UpdateUserProperties(newUserProperties);
            
            return Response<User?>.SuccessUser(null!, ApiMessages.UserUpdated, StatusCodes.Status404NotFound);
        }
        catch (Exception e)
        {
            return Response<User?>.FailWithException(null!, e.Message, ExceptionService.GetStatusCode(e), e.StackTrace!);
        }
    }
}
