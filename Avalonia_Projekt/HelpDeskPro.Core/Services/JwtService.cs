using HelpDeskPro.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HelpDeskPro.Core.Services;

public class JwtService
{
    private readonly string _secretKey;
    private readonly int _expirationMinutes;

    public JwtService(string secretKey, int expirationMinutes = 1440)
    {
        _secretKey = secretKey;
        _expirationMinutes = expirationMinutes;
    }

    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "HelpDeskPro",
            audience: "HelpDeskProClient",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates a JWT token and returns the claims if valid.
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = "HelpDeskPro",
                ValidateAudience = true,
                ValidAudience = "HelpDeskProClient",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
