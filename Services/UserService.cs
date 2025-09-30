using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Response.Entities;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Models.Users.Requests;
using IsekaiFantasyBE.Repository;
using IsekaiFantasyBE.Services.Utils;

namespace IsekaiFantasyBE.Services;

public class UserService
{
    private readonly UserRepository _userRepo;
    private readonly Mailer _emailSenderService;

    public UserService(UserRepository userRepo, Mailer emailSenderService)
    {
        _userRepo = userRepo;
        _emailSenderService = emailSenderService;
    }

    public async Task<ResponseModel> GetMyself(Guid id)
    {
        var user = await _userRepo.GetUserById(id);
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound) 
            : ResponseService.Ok(new Myself(user), ApiMessages.UserRetrieved);
    }

    public async Task<ResponseModel> GetUserById(Guid id)
    {
        var user = await _userRepo.GetUserById(id);
        
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound)
            : ResponseService.Ok(new UserResponse(user), ApiMessages.UserRetrieved);
    }

    public async Task<ResponseModel> GetUserByEmail(string email)
    {
        EmailValidationService.ValidateEmail(email);
        
        var user = await _userRepo.GetUserByEmail(email);
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound) 
            : ResponseService.Ok(new UserResponse(user), ApiMessages.UserRetrieved);
    }

    public async Task<ResponseModel> GetUserByUsername(string username, HttpContext context)
    {
        var user = await _userRepo.GetUserByUsername(username);
        return user is null 
            ? ResponseService.NotFound(ApiMessages.UserNotFound) 
            : ResponseService.Ok(new UserResponse(user), ApiMessages.UserRetrieved);
    }

    public async Task<ResponseModel> PreRegisterUser(UserDTO userDto)
    {
        Credentials.Validate(userDto);

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
            Password = PasswordService.Encrypt(userDto.Password!),
            EmailValidationToken = Credentials.GenerateValidationToken(),
        };

        await _userRepo.PreRegisterUser(preRegister);

        _emailSenderService.SendEmailVerification(preRegister);

        return ResponseService.Ok(preRegister, ApiMessages.UserCreated);
    }
    
    public async Task<ResponseModel> FinishRegisterUser(UserConfirmationDTO dto)
    {
        var preRegister = await _userRepo.GetPreRegisteredUserByToken(dto.Token);

        if (preRegister is null)
        {
            return ResponseService.NotFound(ApiMessages.NotInPreRegister);
        }
        if (!PasswordService.Verify(dto.Password, preRegister.Password))
        { 
            return ResponseService.BadRequest(ApiMessages.WrongPassword);
        }
        
        var user = await _userRepo.FinishRegisterUser(preRegister);

        return user is null 
            ? ResponseService.NotFound(ApiMessages.NotInPreRegister)
            : ResponseService.Created(new UserResponse(user), ApiMessages.UserCreated);
    }
    
    public async Task<ResponseModel> LoginUser(UserDTO userDto)
    {
        Credentials.Validate(userDto);
        var user = await GetUserByHandle(userDto.Username ?? userDto.Email!);
        
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }
        
        if(!PasswordService.Verify(userDto.Password!, user.Password))
        {
            return ResponseService.BadRequest(ApiMessages.WrongPassword);
        }

        if (user.Properties.Status == UserStatus.Banned)
        {
            throw new UnauthorizedAccessException(ApiMessages.UserBanned);
        }

        var token = JwtService.GenerateJwtToken(user);
        return ResponseService.Ok(token, ApiMessages.LoginSuccess);
    }

    public async Task<ResponseModel> UpdateProperties(UserPropertiesDTO properties, HttpContext context)
    {
        var userId = JwtService.GetAuthenticatedUserId(context);
        Console.WriteLine(userId);
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
    
    public async Task<ResponseModel> ResetPasswordRequest(string handle)
    {
        var user = await GetUserByHandle(handle);
        
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }

        var passwordRestRequest = _userRepo.NewPasswordResetRequest(user);
        _emailSenderService.SendPasswordResetEmail(passwordRestRequest);
        
        return ResponseService.Ok(new UserResponse(user.Id, user.Username), ApiMessages.UserUpdated);
    }
    
    private async Task<User?> GetUserByHandle(string handle)
    {
        User? user;
        if (EmailValidationService.ValidateEmail(handle, false))
        {
            user = await _userRepo.GetUserByEmail(handle);
        }
        else
        {
            user = await _userRepo.GetUserByUsername(handle);
        }
        return user;
    }
}
