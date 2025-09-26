// UserControllerTests.cs
using IsekaiFantasyBE.Models.DTO;
using IsekaiFantasyBE.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing; // ✅ Para StatusCodes
using Xunit;

namespace IsekaiFantasyBE.Tests.UserAPI;

public class UserControllerTests : BaseApiTest
{
    public UserControllerTests(WebApplicationFactory<Program> factory) : base(factory) { }
    
    private const string BaseUri = "/Users";

    [Fact]
    public async Task TestPreRegisterWithNoCredentials()
    {
        var userDto = new UserDTO("", "", "");

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.EmptyCredentials, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }

    [Fact]
    public async Task TestRegisterWithInvalidEmail()
    {
        var userDto = new UserDTO(Username: null, Email: "invalid-mail", Password: "string123!S");

        var response = await Client.PostAsync($"{BaseUri}/pre-register", GetContent(userDto));
        var responseModel = await ResponseSerialize(response);

        Assert.NotNull(responseModel);
        Assert.Equal(ApiMessages.EmailInvalid, responseModel.Message);
        Assert.Equal(StatusCodes.Status400BadRequest, responseModel.StatusCode);
    }
}