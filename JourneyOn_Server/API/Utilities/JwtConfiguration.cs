using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace API.Utilities;

[ExcludeFromCodeCoverage]
public static class JwtConfiguration
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {

        var jwtSettings = configuration.GetSection("JWT")
            ?? throw new InvalidOperationException("JWT not found in appsettings.json");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? string.Empty);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"] ?? string.Empty,
                ValidAudience = jwtSettings["Audience"] ?? string.Empty,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}