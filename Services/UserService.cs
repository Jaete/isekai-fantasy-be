using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Repository;
using IsekaiFantasyBE.Services.Utils;

namespace IsekaiFantasyBE.Services;

public class UserService
{
    private UserRepository _userRepo;
    private Mailer _emailSenderService;

    public UserService(UserRepository userRepo, Mailer emailSenderService)
    {
        _userRepo = userRepo;
        _emailSenderService = emailSenderService;
    }

    private static ResponseModel CreateMyselfResponse(User? user)
    {
        if (user is null)
        {
            return ResponseModel.Write(
                null!,
                message: ApiMessages.UserNotFound,
                statusCode: StatusCodes.Status404NotFound
            );
        }

        var myself = new Myself
        {
            Userid = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLogin = user.LastLogin,
            Properties = user.Properties is not null
                ? new()
                {
                    Bio = user.Properties.Bio,
                    Photo = user.Properties.Photo,
                    LastActivity = user.Properties.LastActivity,
                    Status = user.Properties.Status,
                    UserRole = user.Properties.UserRole
                }
                : null
        };

        return ResponseModel.Write(
            myself,
            message: ApiMessages.UserRetrieved,
            statusCode: StatusCodes.Status200OK
        );
    }

    private static ResponseModel CreateSingleUserResponse(User? user)
    {
        if (user is null)
        {
            return ResponseModel.Write(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
        }

        return ResponseModel.Write(user, ApiMessages.UserRetrieved, StatusCodes.Status200OK);
    }

    public async Task<ResponseModel> GetMyself(Guid id)
    {
        try
        {
            var user = await _userRepo.GetUserById(id);
            return CreateMyselfResponse(user);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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
