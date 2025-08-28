using System.Data;
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
        try
        {
            var user = await _userRepo.GetUserById(id);
            return CreateSingleUserResponse(user);
        }
        catch
        {
            throw;
        }
    }
    public async Task<ResponseModel> GetUserByEmail(string email)
    {        
        var user = await _userRepo.GetUserByEmail(email);
        return CreateSingleUserResponse(user);
    }

    public async Task<ResponseModel> GetUserByUsername(string username, HttpContext context)
    {
        var user = await _userRepo.GetUserByUsername(username);
        return CreateSingleUserResponse(user);
    }

    public async Task<ResponseModel> PreRegisterUser(UserDTO userDto)
    {
        try
        {
            if (!Credentials.Validate(userDto))
            {
                throw new ArgumentException(ApiMessages.EmptyCredentials);
            }

            var alreadyRegistered = await _userRepo.GetUserByEmail(userDto.Email!) != null;
            var alreadyInPreRegister = await _userRepo.GetPreRegisteredUserByEmail(userDto.Email!) != null;

            if (alreadyRegistered) {
                return ResponseModel.Write(null!, ApiMessages.AlreadyRegistered, StatusCodes.Status422UnprocessableEntity);
            }

            if (alreadyInPreRegister) {
                return ResponseModel.Write(null!, ApiMessages.InRegisterProgress, StatusCodes.Status422UnprocessableEntity);
            }
            
            var preRegister = new PreRegistrationUser()
            {
                Email = userDto.Email!,
                Username = userDto.Username!,
                Password = Encryption.Encrypt(userDto.Password!),
                EmailValidationToken = Guid.NewGuid(),
            };

            await _userRepo.PreRegisterUser(preRegister);

            _emailSenderService.SendEmailVerification(preRegister);

            return ResponseModel.Write(preRegister, ApiMessages.UserCreated, StatusCodes.Status200OK);

        }
        catch (Exception ex) 
        {
            throw new Exception(ex.Message);
        }

    }

    public async Task<ResponseModel> FinishRegisterUser(UserConfirmationDTO dto)
    {
        try
        {
            var user = await _userRepo.FinishRegisterUser(dto.Token, dto.Password);

            if (user is null)
            {
                return ResponseModel.Write(user!, ApiMessages.NotInPreRegister, StatusCodes.Status404NotFound);
            }

            return ResponseModel.Write(user!, ApiMessages.UserCreated, StatusCodes.Status201Created);
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
            Credentials.Validate(userDto);

            User? user = null;

            if (userDto.Username != null)
            {
                user = await _userRepo.GetUserByUsername(userDto.Username!);
            }
            else
            {
                user = await _userRepo.GetUserByEmail(userDto.Email!);
            }

            if (user is null)
            {
                return ResponseModel.Write(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
            }
            
            if(!Encryption.Verify(userDto.Password!, user.Password))
            {
                return ResponseModel.Write(null!, ApiMessages.WrongPassword, StatusCodes.Status400BadRequest);
            }

            var token = JwtAuth.GenerateJwtToken(user);
            return ResponseModel.Write(token, ApiMessages.LoginSuccess, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ResponseModel> UpdateProperties(UserPropertiesDTO properties, HttpContext context, Guid? id = null)
    {
        try
        {
            Guid userId; 
            if (id is not null)
            {
                JwtAuth.IsAdmin(context);
                userId = (Guid) id;
            }
            else
            {
                JwtAuth.RequireAuthentication(context);
                userId = JwtAuth.GetAuthenticatedUserId(context);
            }

            var user = await _userRepo.GetUserById(userId);
            
            if (user is null)
            {
                return ResponseModel.Write(null!, ApiMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            var newUserProperties = new UserProperties
            {
                User = user,
                LastActivity = DateTime.Now,
                Bio = properties.Bio ?? user.Properties.Bio,
                Photo = properties.Photo ?? user.Properties.Photo,
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
