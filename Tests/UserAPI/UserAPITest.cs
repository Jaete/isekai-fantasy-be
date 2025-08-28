using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using Microsoft.AspNetCore.Http;
using UserAPI.Tests;
using Xunit;

namespace IsekaiFantasyBE.Tests.UserAPI;

public class UserControllerTests : BaseAPITest
{
    private const string BaseUri = "/Users";
    
    [Fact]
    public async Task TestRegisterWithNoCredentials()
    {
        var userDto = new UserDTO
        {
            Username = "",
            Password = "",
            Email = "",
        };

        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.EmptyCredentials, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }

    [Fact]
    public async Task TestRegisterWithInvalidEmail()
    {
        var userDto = new UserDTO
        {
            Username = "test",
            Password = "test", // Password is invalid too, but email validation throws first
            Email = "",
        };

        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.EmailInvalid, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }

    [Fact]
    public async Task TestRegisterWithTooFewCharactersPassword()
    {
        var userDto = new UserDTO
        {
            Username = "test",
            Password = "test",
            Email = "user@example.com",
        };
        
        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidLength, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestRegisterWithMissingUppercasePassword()
    {
        var userDto = new UserDTO
        {
            Username = "test",
            Password = "test!1234",
            Email = "user@example.com",
        };
        
        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidUpper, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestRegisterWithMissingLowercasePassword()
    {
        var userDto = new UserDTO
        {
            Username = "test",
            Password = "TEST!1234",
            Email = "user@example.com",
        };
        
        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidLower, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestRegisterWithMissingDigitPassword()
    {
        var userDto = new UserDTO
        {
            Username = "test",
            Password = "TESTing!!",
            Email = "user@example.com",
        };
        
        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidDigit, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
    
    [Fact]
    public async Task TestRegisterWithMissingSpecialCharPassword()
    {
        var userDto = new UserDTO
        {
            Username = "test",
            Password = "TesT12345",
            Email = "user@example.com",
        };
        
        var response = await Client.PostAsync($"{BaseUri}/register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);
        
        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.PasswordInvalidSpecial, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
}