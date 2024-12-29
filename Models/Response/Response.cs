using System.Diagnostics;
using IsekaiFantasyBE.Models.Users;
using Mysqlx.Datatypes;
using OneOf;

namespace IsekaiFantasyBE.Models.Response;

public class Response<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    
    public string? StackTrace { get; set; }
    
    public Response(){}

    private Response(T value, string message, int statusCode, string? stack = null)
    {
        Data = value;
        Message = message;
        StatusCode = statusCode;
    }
    
    public static Response<Guid> Success(Guid value, string message, int statusCode) => new(value, message, statusCode);

    public static Response<OneOf<User, string>?> Success(OneOf<User, string> value, string message, int statusCode) => new(value, message, statusCode);
    
    public static Response<OneOf<User, string>?> Fail(OneOf<User, string> value, string message, int statusCode) => new(value, message, statusCode);
    
    public static  Response<OneOf<User, string>?> FailWithException(OneOf<User, string> value, string message, int statusCode, string stack) => new(value, message, statusCode, stack);
    
    public static Response<string> Success(string value, string message, int statusCode) => new(value, message, statusCode);
    
    public static Response<string> FailString(string value, string message, int statusCode) => new(value, message, statusCode);
    
    public static  Response<string> FailStringWithException(string value, string message, int statusCode, string stack) => new(value, message, statusCode, stack);
}