using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using Microsoft.IdentityModel.Tokens;
using Exception = System.Exception;

namespace IsekaiFantasyBE.Services.Utils;

public class JwtAuth
{
    private static string _secret;

    public static void Initialize(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"];
    }

    public static string GenerateJwtToken(User user)
    {
        if (string.IsNullOrEmpty(_secret))
        {
            throw new InvalidOperationException("Secret not initialized.");
        }

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Properties.UserRole.ToString() ?? UserProperties.Role.Member.ToString())
        };

        var jwtToken = new JwtSecurityToken(
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
        if (!context.Request.Headers.ContainsKey("Authorization") || string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
        {
            throw new AuthenticationException(ApiMessages.NotAuthenticated);
        }
    }

    public static bool IsAdmin(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization") || string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
        {
            throw new AuthenticationException(ApiMessages.NotAuthenticated);
        }

        var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];
        var handler = new JwtSecurityTokenHandler();
        var tokenS = handler.ReadToken(token) as JwtSecurityToken;

        var roleClaim = tokenS?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == UserProperties.Role.Admin.ToString();
    }

    public static Guid GetAuthenticatedUserId(HttpContext context)
    {
        try
        {
            if (context.Request.Headers.ContainsKey("Authorization") == false || string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
            {
                throw new AuthenticationException(ApiMessages.NotAuthenticated);
            }
            var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(id);
        }
        catch
        {
            throw;
        }

    }
}