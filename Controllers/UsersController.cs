using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Services;
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
    [Route("id/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response<User>>> GetById(Guid id)
    {
        var users = await _userService.GetUserById(id);

        return users.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(users),
            StatusCodes.Status404NotFound => NotFound(users),
            _ => Ok(users)
        };
    }
    
    [HttpGet]
    [Route("email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response<User>>> GetByEmail(string email)
    {
        var user = await _userService.GetUserByEmail(email);

        return user.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(user),
            StatusCodes.Status404NotFound => NotFound(user),
            _ => Ok(user)
        };
    }
    
    [HttpGet]
    [Route("username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Response<User>>> GetByUsername(string username)
    {
        var user = await _userService.GetUserByUsername(username);

        return user.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(user),
            StatusCodes.Status404NotFound => NotFound(user),
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
        if (response.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest(response);
        }

        if (response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        var user = response.Data as User;
        return Ok(Response<Guid>.Success(user!.Id, ApiMessages.UserCreated, StatusCodes.Status201Created));
    }
}