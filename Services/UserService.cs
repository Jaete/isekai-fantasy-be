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
            : ResponseService.Ok(new UserResponse(user), ApiMessages.UserRetrieved);
    }

    public async Task<ResponseModel> GetUserByEmail(string email)
    {
        EmailValidationService.IsValidEmail(email);
        
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
            Password = Encryption.Encrypt(userDto.Password!),
            EmailValidationToken = Credentials.GenerateEmailValidationToken(),
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
            : ResponseService.Created(new UserResponse(user), ApiMessages.UserCreated);
    }
    
    public async Task<ResponseModel> LoginUser(UserDTO userDto)
    {
        Credentials.Validate(userDto);
        var user = userDto.Username != null 
            ? await _userRepo.GetUserByUsername(userDto.Username!)
            : await _userRepo.GetUserByEmail(userDto.Email!);
        
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }
        
        if(!PasswordService.Verify(userDto.Password!, user.Password))
        {
            return ResponseService.BadRequest(ApiMessages.WrongPassword);
        }

        if (user.Properties!.Status == UserStatus.Banned)
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
    
    private static ResponseModel CreateMyselfResponse(User? user)
    {
        if (user is null)
        {
            return ResponseService.NotFound(ApiMessages.UserNotFound);
        }
        
        return ResponseService.Ok(
            new Myself(user),
            message: ApiMessages.UserRetrieved
        );
    }
}
