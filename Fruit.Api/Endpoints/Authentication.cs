using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fruit.Api.Endpoints;

public static class Authentication
{
    public static void MapAuthication(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", (IConfiguration config) =>
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
    }
}
