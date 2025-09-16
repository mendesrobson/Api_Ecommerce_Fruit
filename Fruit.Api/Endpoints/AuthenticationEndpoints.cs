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
        // Endpoint genérico para gerar um token (pode ser removido se não for usado)
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
            return Results.Ok(new { token = jwtToken });
        });

        // Endpoint de refresh token
        app.MapPost("/api/refresh", (RefreshTokenRequest request, IConfiguration config) =>
        {
            // 1. Valida se o refresh token existe no nosso "banco de dados" temporário.
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
                ValidateLifetime = false // Importante: não valida o tempo de vida aqui.
            };

            ClaimsPrincipal principal;
            try
            {
                // A validação irá falhar se a assinatura ou o emissor/audiência estiverem incorretos.
                principal = tokenHandler.ValidateToken(request.ExpiredToken, tokenValidationParameters, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                // Se a validação falhar por qualquer motivo (exceto tempo de vida), retorna Unauthorized.
                return Results.Unauthorized();
            }

            // 3. Invalida o refresh token antigo para evitar reuso.
            refreshTokens.TryRemove(request.RefreshToken, out _);

            // 4. Extrai o ID do usuário (subject) do token.
            // Correção: Usa ClaimTypes.NameIdentifier para garantir que o 'sub' seja encontrado.
            var mobileNo = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(mobileNo))
            {
                // Se o token não contiver o ID do usuário, a validação é inválida.
                return Results.Unauthorized();
            }

            // 5. Gera um novo Access Token e um novo Refresh Token.
            var newJwtToken = GenerateJwtToken(mobileNo, config);
            var newRefreshToken = Guid.NewGuid().ToString();

            // 6. Armazena o novo refresh token para o usuário.
            refreshTokens.TryAdd(newRefreshToken, mobileNo);

            return Results.Ok(new { accessToken = newJwtToken, refreshToken = newRefreshToken });
        }).WithOpenApi();
    }

    // Método auxiliar para gerar o JWT Token.
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

    // Método para adicionar um refresh token (chamado a partir do endpoint de login).
    public static void AddRefreshToken(string refreshToken, string mobileNo)
    {
        refreshTokens.TryAdd(refreshToken, mobileNo);
    }
}