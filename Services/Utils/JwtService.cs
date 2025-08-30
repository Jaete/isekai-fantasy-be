using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Exception = System.Exception;

namespace IsekaiFantasyBE.Services.Utils;

public class JwtService
{
    private static string _secret;
    private static string _issuer;
    private static string _audience;
    
    public static void Initialize(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
        
        Console.WriteLine($"[JWT INIT] Secret: '{_secret?.Length} chars'");
        Console.WriteLine($"[JWT INIT] Issuer: '{_issuer}'");
        Console.WriteLine($"[JWT INIT] Audience: '{_audience}'");

        if (string.IsNullOrEmpty(_secret)) throw new InvalidOperationException("JWT Secret not configured.");
        if (string.IsNullOrEmpty(_issuer)) throw new InvalidOperationException("JWT Issuer not configured.");
        if (string.IsNullOrEmpty(_audience)) throw new InvalidOperationException("JWT Audience not configured.");
    }

    public static string GenerateJwtToken(User user)
    {
        if (string.IsNullOrEmpty(_secret))
        {
            throw new InvalidOperationException("Secret not initialized.");
        }

        var claims = new List<Claim> {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.Role, user.Properties?.UserRole!.Value.GetDisplayName() ?? nameof(UserProperties.Role.Member)),
        };

        var jwtToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience, 
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_secret)
                ),
                SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
    public static void RequireAuthentication(HttpContext context)
    {
        if (context.Request.Headers.Authorization.Count == 0)
        {
            throw new AuthenticationException(ApiMessages.NotAuthenticated);
        }
    }

    public static void RequireAdminAccess(HttpContext context)
    {
        RequireAuthentication(context);
        if (!context.Request.HttpContext.User.IsInRole(nameof(UserProperties.Role.Admin)))
        {
            throw new UnauthorizedAccessException(ApiMessages.InsufficientPermissions);
        }
    }
    
    public static Guid GetAuthenticatedUserId(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.ToString().Split(" ")[1];
        var handler = new JwtSecurityTokenHandler();
        var tokenS = handler.ReadToken(token) as JwtSecurityToken;
        var id = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        return Guid.Parse(id);        
    }
}