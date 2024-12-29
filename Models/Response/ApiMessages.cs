using System.Xml;

namespace IsekaiFantasyBE.Models.Response;

public class ApiMessages
{
    public const string UserRetrieved = "User(s) retrieved successfully";
    public const string UserNotFound = "User(s) not found";
    public const string UserCreated = "User registered successfully";
    public const string EmailInvalid = "Email is invalid";
    public const string EmailAlreadyExists = "Email already exists";
    public const string PasswordInvalidLength = "Password must be at least 8 characters long.";
    public const string PasswordInvalidUpper = "Password must have at least one upper case letter.";
    public const string PasswordInvalidLower = "Password must have at least one lower case letter.";
    public const string PasswordInvalidDigit = "Password must have at least one digit.";
    public const string PasswordInvalidSpecial = "Password must have at least one special character.";
    public const string EmptyCredentials = "Username and password must not be empty.";
    public const string WrongPassword = "Password is incorrect.";
    public const string LoginSuccess = "Login successful.";
    public const string NotAuthenticated = "Unauthorized. You must be logged in.";
    public const string InsufficientPermissions = "Unauthorized. Insufficient permissions.";
    public const string UserUpdated = "User updated successfully.";
}