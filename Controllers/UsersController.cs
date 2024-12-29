using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;

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
    [Route("id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<User?>>> GetById(string id)
    {
        JwtService.RequireAuthentication(HttpContext);
        var guid = Guid.Parse(id);
        var users = await _userService.GetUserById(guid, HttpContext);

        return users.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(users),
            StatusCodes.Status404NotFound => NotFound(users),
            StatusCodes.Status401Unauthorized => Unauthorized(users),
            StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, users),
            _ => Ok(users)
        };
    }
    
    [HttpGet]
    [Route("email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<User?>>> GetByEmail(string email)
    {
        JwtService.RequireAuthentication(HttpContext);
        var user = await _userService.GetUserByEmail(email, HttpContext);

        return user.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(user),
            StatusCodes.Status404NotFound => NotFound(user),
            StatusCodes.Status401Unauthorized => Unauthorized(user),
            StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, user),
            _ => Ok(user)
        };
    }

    [HttpGet]
    [Route("username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Response<User?>>> GetByUsername(string username)
    {
        
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
    
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<Guid>>> Register([FromBody] UserDTO userDto)
    {
        var response = await _userService.RegisterNewUser(userDto);
        var user = response.Data;
        return response.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(response),
            StatusCodes.Status404NotFound => NotFound(response),
            StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
            _ => Ok(Response<Guid>.SuccessGuid(user!.Id, ApiMessages.UserCreated, StatusCodes.Status201Created)),
        };
        
    }
    
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<string>>> Login([FromBody] UserDTO userDto)
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

    [HttpPut]
    [Route("update/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<User?>>> UpdateProperties(string id, [FromBody] UserPropertiesDTO userProperties)
    {
        var response = await _userService.UpdateProperties(userProperties, HttpContext);
        return response.StatusCode switch
        {
            StatusCodes.Status401Unauthorized => Unauthorized(response),
            StatusCodes.Status400BadRequest => BadRequest(response),
            StatusCodes.Status404NotFound => NotFound(response),
            StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, response),
            _ => Ok(response),
        };
    }
}