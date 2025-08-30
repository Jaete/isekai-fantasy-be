using IsekaiFantasyBE.Models.Response;

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

    public static ResponseModel UnprocessableEntity(string message)
    {
        return ResponseModel.Write(null!, message, StatusCodes.Status422UnprocessableEntity);
    }

    public static ResponseModel InternalError(string message, int statusCode, string stackTrace)
    {
        return ResponseModel.Write(null!, message, statusCode, stackTrace);        
    }
}