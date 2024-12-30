namespace IsekaiFantasyBE.Models.Response;

public class ResponseModel
{
    public object? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public string? StackTrace { get; set; }

    public static ResponseModel Write(object value, string message, int statusCode, string? stack = null)
    {
        return new ResponseModel
        {
            Data = value,
            Message = message,
            StatusCode = statusCode,
            StackTrace = stack,
        };
    }
}