using System.Text;
using System.Text.Json;
using IsekaiFantasyBE.Models.Response;
using Microsoft.Extensions.Configuration;

namespace UserAPI.Tests;

public class BaseAPITest
{

    protected HttpClient Client;
    
    public BaseAPITest()
    {
        Client = SetupClient();
    }
    private static string GetUri()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        return configuration["Api:BaseAddress"];
    }
    
    protected static HttpClient SetupClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };

        var client = new HttpClient(handler);
        client.BaseAddress = new Uri(GetUri());
        
        return client;
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
}