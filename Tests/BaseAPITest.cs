using System.Text;
using System.Text.Json;
using IsekaiFantasyBE.Contexts;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IsekaiFantasyBE.Tests;

public class BaseApiTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly WebApplicationFactory<Program> _factory;

    protected BaseApiTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        ResetDatabase();
        Client = factory.CreateClient();
    }
    
    protected static async Task<ResponseModel?> ResponseSerialize(HttpResponseMessage response)
    {
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ResponseModel>(
            responseJson, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }
    
    protected static StringContent GetContent(object obj)
    {
        return new StringContent(
            JsonSerializer.Serialize(obj),
            Encoding.UTF8,
            "application/json"
        );
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    protected async Task<T?> FindEntityAsync<T>(object id) where T : class
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
        return await context.Set<T>().FindAsync(id);
    }
}