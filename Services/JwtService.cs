using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IsekaiFantasyBE.Models.Users;
using Microsoft.IdentityModel.Tokens;

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
}