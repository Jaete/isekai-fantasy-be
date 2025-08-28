using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Services;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IsekaiFantasyBE.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly AdminService  _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPut]
    [Route("update/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> UpdateUserProperties(string id, [FromBody] UserPropertiesDTO userProperties)
    {
        try
        {
            JwtService.RequireAdminAccess(HttpContext);
            var guid = Guid.Parse(id);
            var response = await _adminService.UpdateUserProperties(guid, userProperties);
        
            return response.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                StatusCodes.Status403Forbidden => Forbid(response.Message ?? ApiMessages.InsufficientPermissions),
                _ => Ok(response)
            };
        }
        catch (Exception e)
        {
            var statusCode = ExceptionService.GetStatusCode(e);
            return StatusCode(
                statusCode,
                ResponseService.InternalError(e.Message, statusCode, e.StackTrace!)
            );
        }
    }
    
    [HttpPut]
    [Route("ban/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseModel>> Ban(string id, [FromBody]BanUserDTO banUserProps)
    {
        try
        {
            JwtService.RequireAdminAccess(HttpContext);
            var userId = Guid.Parse(id);

            var response = await _adminService.BanUser(userId, banUserProps, HttpContext);
            return response.StatusCode switch
            {
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status403Forbidden => Forbid(response.Message ?? ApiMessages.InsufficientPermissions),
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
                ResponseService.InternalError(e.Message, statusCode, e.StackTrace!)
            );
        }
        
    }
}