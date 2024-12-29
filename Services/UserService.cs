using System.Diagnostics;
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
    
    private static Response<OneOf<User, string>?> CreateSingleUserResponse(User? user)
    {
        if (user is null)
        {
            return Response<OneOf<User, string>>.Fail("", ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
        }

        return Response<OneOf<User, string>>.Success(user, ApiMessages.UserRetrieved, StatusCodes.Status200OK);
    }
    
    private static Response<OneOf<User, string>?> CreateEmailInvalidResponse(string email)
    {
        return Response<OneOf<User, string>>.Fail("", ApiMessages.EmailInvalid, StatusCodes.Status400BadRequest)!;
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

    public async Task<Response<OneOf<User, string>?>> GetUserById(Guid id)
    {
        var user = await _userRepo.GetUserById(id);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<OneOf<User, string>?>> GetUserByEmail(string email)
    {
        if (!EmailValidationService.IsValidEmail(email))
        {
            return CreateEmailInvalidResponse(email);
        }
        
        var user = await _userRepo.GetUserByEmail(email);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<OneOf<User, string>?>> GetUserByUsername(string username)
    {
        var user = await _userRepo.GetUserByUsername(username);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<OneOf<User, string>?>> RegisterNewUser(UserDTO userDto)
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
            return Response<OneOf<User, string>?>.Success(user, ApiMessages.UserCreated, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return Response<OneOf<User, string>?>.FailWithException("", ex.Message, ExceptionService.GetStatusCode(ex), ex.StackTrace!);
        }
    }
    
    public async Task<Response<string>?> LoginUser(UserDTO userDto)
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
            return Response<string>.Success(token, ApiMessages.LoginSuccess, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return Response<string>.FailStringWithException("", e.Message, ExceptionService.GetStatusCode(e), e.StackTrace!);
        }
    }
    
    
}
