using System.Diagnostics;
using IsekaiFantasyBE.Models.Users;
using Mysqlx.Datatypes;

namespace IsekaiFantasyBE.Models.Response;

public class Response<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    
    public string? StackTrace { get; set; }

    private Response(T value, string message, int statusCode, string? stack = null)
    {
        Data = value;
        Message = message;
        StatusCode = statusCode;
    }

    public static Response<T> Success(T value, string message, int statusCode) => new(value, message, statusCode);
    
    public static Response<T> Fail(T value, string message, int statusCode) => new(value, message, statusCode);
    
    public static Response<T> FailWithException(T value, string message, int statusCode, string stack) => new(value, message, statusCode, stack);
}