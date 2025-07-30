using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public interface IAccessTokenService
{
    Task<Dictionary<string, string?>> GenerateTokensAsync(ApplicationUser user);
}
public sealed class AccessTokenService (IConfiguration configuration, UserManager<ApplicationUser>
        userManager)
    : IAccessTokenService
{
    public async Task<Dictionary<string, string?>> GenerateTokensAsync(ApplicationUser user)
    {
        var jwtSecretKey = !string.IsNullOrEmpty(configuration["JWT:Key"])
            ? configuration["JWT:Key"]
            : throw new InvalidOperationException("JWT:Key not found in configuration.");
        var jwtIssuer = !string.IsNullOrEmpty(configuration["JWT:Issuer"])
            ? configuration["JWT:Issuer"]
            : throw new InvalidOperationException("JWT:Issuer not found in configuration.");
        var jwtAudience = !string.IsNullOrEmpty(configuration["JWT:Audience"])
            ? configuration["JWT:Audience"]
            : throw new InvalidOperationException("JWT:Audience not found in configuration.");

        var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();

        // Define the token's claims.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", user.Id.ToString()),
            new Claim("role", userRole ?? throw new InvalidOperationException("Failed to retrieve user role for the specified user.")),
            // Add additional claims as needed.
        };

        // Get the secret key from the configuration.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the token.
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Return the tokens as a dictionary.
        return new Dictionary<string, string?>
        {
            {"user_id", user.Id.ToString()},
            { "access_token", accessToken },
            { "token_expiration", DateTime.UtcNow.AddMinutes(30).ToString("o")},
        };
    }
}