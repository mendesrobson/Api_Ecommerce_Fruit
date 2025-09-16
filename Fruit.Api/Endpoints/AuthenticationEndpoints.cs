using Fruit.Api.Domain.Token;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fruit.Api.Endpoints;

public static class AuthenticationEndpoints
{
    private static readonly ConcurrentDictionary<string, string> refreshTokens = new();

    // Método para mapear todos os endpoints de autenticação
    public static void MapAuthentication(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/token", (IConfiguration config) =>
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "user_id_123"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();
            AddRefreshToken(refreshToken, jwtToken);
            return Results.Ok(new { token = jwtToken, refreshToken  });
        });

        // Endpoint de refresh token
        app.MapPost("/api/refresh", (RefreshTokenRequest request, IConfiguration config) =>
        {
            if (string.IsNullOrEmpty(request.RefreshToken) || !refreshTokens.ContainsKey(request.RefreshToken))
            {
                return Results.Unauthorized();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = config["Jwt:Audience"],
                ValidateLifetime = false 
            };

            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(request.ExpiredToken, tokenValidationParameters, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                return Results.Unauthorized();
            }

            refreshTokens.TryRemove(request.RefreshToken, out _);

            var mobileNo = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(mobileNo))
            {
                return Results.Unauthorized();
            }

             var newJwtToken = GenerateJwtToken(mobileNo, config);
            var newRefreshToken = Guid.NewGuid().ToString();

            refreshTokens.TryAdd(newRefreshToken, mobileNo);

            return Results.Ok(new { accessToken = newJwtToken, refreshToken = newRefreshToken });
        }).WithOpenApi();
    }

    public static string GenerateJwtToken(string mobileNo, IConfiguration config)
    {
        var claims = new[]
        {
            new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, mobileNo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15), // Validade curta (15 minutos).
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static void AddRefreshToken(string refreshToken, string mobileNo)
    {
        refreshTokens.TryAdd(refreshToken, mobileNo);
    }
}