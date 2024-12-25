using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Repository;

namespace IsekaiFantasyBE.Services;

public class UserService
{
    private UserRepository _userRepo;

    public UserService(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }
    
    private static Response<User> CreateSingleUserResponse(User? user)
    {
        if (user is null)
        {
            return Response<User>.Fail(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
        }

        return Response<User>.Success(user, ApiMessages.UserRetrieved, StatusCodes.Status200OK);
    }
    
    private static Response<User> CreateEmailInvalidResponse(string email)
    {
        return Response<User>.Fail(null!, ApiMessages.EmailInvalid, StatusCodes.Status400BadRequest)!;
    }

    public async Task<Response<User>> GetUserById(Guid id)
    {
        var user = await _userRepo.GetUserById(id);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<User>> GetUserByEmail(string email)
    {
        if (!EmailValidationService.IsValidEmail(email))
        {
            return CreateEmailInvalidResponse(email);
        }
        
        var user = await _userRepo.GetUserByEmail(email);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<User>> GetUserByUsername(string username)
    {
        var user = await _userRepo.GetUserByUsername(username);
        return CreateSingleUserResponse(user);
    }

    public async Task<Response<object>> RegisterNewUser(UserDTO userDto)
    {
        try
        {
            PasswordService.Validate(userDto.Password);
            
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                Password = PasswordService.Encrypt(userDto.Password),
            };
            await _userRepo.RegisterNewUser(user);
            return Response<object>.Success(user, ApiMessages.UserCreated, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return Response<object>.FailWithException(ex.InnerException!, ex.Message, ExceptionService.GetStatusCode(ex), ex.StackTrace!);
        }
    }
    
    
}
