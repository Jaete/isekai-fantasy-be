// UserControllerTests.cs

using System.Text.Json;
using System.Threading.Tasks;
using IsekaiFantasyBE.Contexts;
using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IsekaiFantasyBE.Tests.UserAPI;

public class UserControllerTests : BaseApiTest
{
    public UserControllerTests(WebApplicationFactory<Program> factory) : base(factory) { }
    
    private const string BaseUri = "/Users";
    private const string TestUsername = "Test User";
    
    private struct TestPasswords
    {
        public const string Valid = "string123!SS";
        public const string TooShort = "S1!s";
        public const string MissingDigit = "string!SS";
        public const string MissingSpecial = "string123SS";
        public const string MissingLower = "STRING123!SS";
        public const string MissingUpper = "string123!ss";
        public const string ValidWrongPassword = "string12345!SS";
    }

    private struct TestEmails
    {
        public const string Valid = "testmail@mail.com";
        public const string Invalid = "invalid-mail";
    }

    [Fact]
    public async Task TestPreRegisterNoCredentials()
    {
        var userDto = new UserDTO(string.Empty, string.Empty, string.Empty);

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.EmptyCredentials, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }

    [Fact]
    public async Task TestPreRegisterInvalidEmail()
    {
        var userDto = new UserDTO(
            Username: string.Empty,
            Email: TestEmails.Invalid,
            Password: TestPasswords.Valid
        );

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.EmailInvalid, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestPreRegisterInvalidPasswordLength()
    {
        var userDto = new UserDTO(
            Username: string.Empty,
            Email: TestEmails.Valid,
            Password: TestPasswords.TooShort 
        );

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidLength, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestPreRegisterInvalidPasswordMissingDigit()
    {
        var userDto = new UserDTO(
            Username: string.Empty,
            Email: TestEmails.Valid,
            Password: TestPasswords.MissingDigit
        );

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidDigit, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestPreRegisterInvalidPasswordMissingSpecialCharacter()
    {
        var userDto = new UserDTO(
            Username: string.Empty,
            Email: TestEmails.Valid,
            Password: TestPasswords.MissingSpecial 
        );

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidSpecial, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestPreRegisterInvalidPasswordMissingLowercase()
    {
        var userDto = new UserDTO(
            Username: string.Empty,
            Email: TestEmails.Valid,
            Password: TestPasswords.MissingLower
        );

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidLower, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestPreRegisterInvalidPasswordMissingUppercase()
    {
        var userDto = new UserDTO(
            Username: string.Empty,
            Email: TestEmails.Valid,
            Password: TestPasswords.MissingUpper
        );

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidUpper, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    
    [Fact]
    public async Task TestPreRegisterSuccess()
    {
        var userDto = new UserDTO(
            Username: TestUsername,
            Email: TestEmails.Valid, 
            Password: TestPasswords.Valid 
        );
        
        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.UserCreated, responseModel.Message);
        Assert.Equal(StatusCodes.Status201Created, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestFinishRegisterWrongPassword()
    {
        var userDto = new UserDTO(
            Username: TestUsername,
            Email: TestEmails.Valid, 
            Password: TestPasswords.Valid 
        );
        
        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.UserCreated, responseModel.Message);
        Assert.Equal(StatusCodes.Status201Created, responseModel.StatusCode);
        
        var id = Guid.Parse(((JsonElement)responseModel.Data).GetString());
        var preRegUser = await FindEntityAsync<PreRegistrationUser>(id);
        Assert.NotNull(preRegUser);
        var emailToken = preRegUser.EmailValidationToken;
        
        var userConfirmation = new UserConfirmationDTO(
            Token: emailToken,
            Password: TestPasswords.ValidWrongPassword 
        );
        
        response = await Client.PostAsync($"{BaseUri}/finish-register", GetContent(userConfirmation));
        responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.WrongPassword, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestFinishRegisterSuccess()
    {
        var userDto = new UserDTO(
            Username: TestUsername,
            Email: TestEmails.Valid, 
            Password: TestPasswords.Valid 
        );
        
        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.UserCreated, responseModel.Message);
        Assert.Equal(StatusCodes.Status201Created, responseModel.StatusCode);

        var id = Guid.Parse(((JsonElement)responseModel.Data).GetString());
        var preRegUser = await FindEntityAsync<PreRegistrationUser>(id);
        Assert.NotNull(preRegUser);
        var emailToken = preRegUser.EmailValidationToken;
        
        var userConfirmation = new UserConfirmationDTO(
            Token: emailToken,
            Password: TestPasswords.Valid 
        );
        
        response = await Client.PostAsync($"{BaseUri}/finish-register", GetContent(userConfirmation));
        responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(StatusCodes.Status201Created, responseModel.StatusCode);
        Assert.Equal(ApiMessages.UserCreated, responseModel.Message);
    }
    
    [Fact]
    
}