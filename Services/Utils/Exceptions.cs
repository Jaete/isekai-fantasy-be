using System.Data;
using System.Security.Authentication;

namespace IsekaiFantasyBE.Services.Utils;

public static class Exceptions
{
    public static int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            FormatException => StatusCodes.Status400BadRequest,
            DuplicateNameException => StatusCodes.Status422UnprocessableEntity,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            AuthenticationException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}