using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IsekaiFantasyBE.Models.Response;
using IsekaiFantasyBE.Models.Users;
using Microsoft.IdentityModel.Tokens;
using OneOf;

namespace IsekaiFantasyBE.Services;

public class JwtService
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
    public static Response<User?> RequireAuthentication(HttpContext context)
    {
        if (context.Request.Headers.Authorization.IsNullOrEmpty())
        {
            return Response<User?>.FailUser(null!, ApiMessages.NotAuthenticated, StatusCodes.Status401Unauthorized);
        }

        return new Response<User?>();
    }
    
    public static Guid GetAuthenticatedUserId(HttpContext context)
    {
        try
        {
            var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }
}