namespace IsekaiFantasyBE.Services;

public static class ExceptionService
{
    public static int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    } 
}