using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Services;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.AspNetCore.Mvc;

namespace IsekaiFantasyBE.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    
    public UsersController(UserService userService)
    {
        _userService = userService;    
    }

    [HttpGet]
    [Route("self")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> GetSelf()
    {
        try
        {
            var response = await _userService.GetMyself(JwtService.GetAuthenticatedUserId(HttpContext));
            return response.StatusCode switch
            {
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
                _ => Ok(response)
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException.Message, statusCode, e.StackTrace)
            );
        }
    }

    [HttpGet]
    [Route("id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> GetById(string id)
    {
        try
        {
            JwtService.RequireAuthentication(HttpContext);
            var guid = Guid.Parse(id);
            var users = await _userService.GetUserById(guid);

            return users.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(users),
                StatusCodes.Status404NotFound => NotFound(users),
                StatusCodes.Status401Unauthorized => Unauthorized(ResponseModel.Write(guid, ApiMessages.NotAuthenticated, StatusCodes.Status401Unauthorized)),
                _ => Ok(users)
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException?.Message!, statusCode, e.InnerException?.StackTrace!)
            );
        }
        
    }
    
    [HttpGet]
    [Route("email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> GetByEmail(string email)
    {
        try
        {
            JwtService.RequireAuthentication(HttpContext);
            var user = await _userService.GetUserByEmail(email);

            return user.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(user),
                StatusCodes.Status404NotFound => NotFound(user),
                StatusCodes.Status401Unauthorized => Unauthorized(user),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, user),
                _ => Ok(user)
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException?.Message!, statusCode, e.InnerException?.StackTrace!)
            );
        }
    }

    [HttpGet]
    [Route("username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseModel>> GetByUsername(string username)
    {
        try
        {
            JwtService.RequireAuthentication(HttpContext);
            var user = await _userService.GetUserByUsername(username, HttpContext);
            return user.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(user),
                StatusCodes.Status404NotFound => NotFound(user),
                StatusCodes.Status401Unauthorized => Unauthorized(user),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, user),
                _ => Ok(user)
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException?.Message!, statusCode, e.InnerException?.StackTrace!)
            );
        }
    }
    
    [HttpPost]
    [Route("pre-register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> PreRegister([FromBody] UserDTO userDto)
    {
        try
        {
            var response = await _userService.PreRegisterUser(userDto);
            var user = response.Data as PreRegistrationUser;
            return response.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status422UnprocessableEntity => UnprocessableEntity(response),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
                _ => Ok(ResponseModel.Write(user!.Id, ApiMessages.UserCreated, StatusCodes.Status201Created)),
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.Message, statusCode, e.StackTrace)
            );
        }
    }

    [HttpPost]
    [Route("finish-register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> FinishRegister([FromBody] UserConfirmationDTO dto)
    {
        try
        {
            var response = await _userService.FinishRegisterUser(dto);
            var user = response.Data as User;
            return response.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
                _ => Ok(ResponseModel.Write(user!.Id, ApiMessages.UserCreated, StatusCodes.Status201Created)),
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException?.Message!, statusCode, e.InnerException?.StackTrace!)
            );
        }
    }
    
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> Login([FromBody] UserDTO userDto)
    {
        try
        {
            var response = await _userService.LoginUser(userDto);

            return response.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
                _ => Ok(response),
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException?.Message!, statusCode, e.InnerException?.StackTrace!)
            );
        }
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> UpdateProperties([FromBody] UserPropertiesDTO userProperties)
    {
        try
        {
            var response = await _userService.UpdateProperties(userProperties, HttpContext);
            return response.StatusCode switch
            {
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
                _ => Ok(response)
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.InnerException?.Message!, statusCode, e.InnerException?.StackTrace!)
            );
        }
    }
}