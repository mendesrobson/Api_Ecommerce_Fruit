namespace Fruit.Api.Domain.Token;

public record class RefreshTokenRequest
{
    public string? RefreshToken { get; set; }
    public string? ExpiredToken { get; set; }
}
