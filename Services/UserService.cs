using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Response.Entities;
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
        EmailValidationService.IsValidEmail(email);
        
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

    public async Task<ResponseModel> PreRegisterUser(UserDTO userDto)
    {
        if (!Credentials.Validate(userDto))
        {
            throw new ArgumentException(ApiMessages.EmptyCredentials);
        }

        var alreadyRegistered = await _userRepo.GetUserByEmail(userDto.Email!) != null;
        var alreadyInPreRegister = await _userRepo.GetPreRegisteredUserByEmail(userDto.Email!) != null;

        if (alreadyRegistered || alreadyInPreRegister)
        {
            return ResponseService.UnprocessableEntity(
                alreadyRegistered 
                    ? ApiMessages.AlreadyRegistered 
                    : ApiMessages.InRegisterProgress
            );
        }
        
        var preRegister = new PreRegistrationUser
        {
            Email = userDto.Email!,
            Username = userDto.Username!,
            Password = Encryption.Encrypt(userDto.Password!),
            EmailValidationToken = Guid.NewGuid(),
        };

        await _userRepo.PreRegisterUser(preRegister);

        _emailSenderService.SendEmailVerification(preRegister);

        return ResponseService.Ok(preRegister, ApiMessages.UserCreated);
    }
    
    public async Task<ResponseModel> FinishRegisterUser(UserConfirmationDTO dto)
    {
        var user = await _userRepo.FinishRegisterUser(dto.Token, dto.Password);

        return user is null 
            ? ResponseService.NotFound(ApiMessages.NotInPreRegister)
            : ResponseService.Created(user, ApiMessages.UserCreated);
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
    
    
    
    private static ResponseModel CreateMyselfResponse(User? user)
    {
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }

        var myself = new Myself
        (
            user.Id,
            user.Username,
            user.Email,
            user.Properties is not null
                ? new UserProperties
                {
                    Bio = user.Properties.Bio,
                    Photo = user.Properties.Photo,
                    LastActivity = user.Properties.LastActivity,
                    Status = user.Properties.Status,
                    UserRole = user.Properties.UserRole
                }
                : null,
            user.CreatedAt,
            user.UpdatedAt,
            user.LastLogin
        );

        return ResponseService.Ok(
            myself,
            message: ApiMessages.UserRetrieved
        );
    }
    
    private static void ValidateEmptyCredentials(UserDTO user)
    {
        if (user.Username is null || user.Password is null)
        {
            throw new ArgumentException(ApiMessages.EmptyCredentials);
        }
    }

    private static void ValidateCredentials(UserDTO userDto)
    {
        ValidateEmptyCredentials(userDto);
        EmailValidationService.IsValidEmail(userDto.Email);
        PasswordService.Validate(userDto.Password);
    }
}
