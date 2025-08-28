using System.Diagnostics;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Response.Entities;
using IsekaiFantasyBE.Models.Users;
using OneOf.Types;

namespace IsekaiFantasyBE.Services;

public class ResponseService
{
    public static ResponseModel Ok(object data, string message)
    {
        return ResponseModel.Write(
            data,
            message,
            StatusCodes.Status200OK
        );
    }

    public static ResponseModel Created(object data, string message)
    {
        return ResponseModel.Write(
            data,
            message, 
            StatusCodes.Status201Created
        );
    }
    
    public static ResponseModel BadRequest(string message)
    {
        return ResponseModel.Write(null!, message, StatusCodes.Status400BadRequest); 
    }

    public static ResponseModel NotFound(string message)
    {
        return ResponseModel.Write(null!, message, StatusCodes.Status404NotFound);
    }

    public static ResponseModel InternalError(string message, int statusCode, string stackTrace)
    {
        return ResponseModel.Write(null!, message, statusCode, stackTrace);        
    }
}