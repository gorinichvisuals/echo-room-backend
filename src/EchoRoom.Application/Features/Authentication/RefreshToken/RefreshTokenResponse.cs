namespace EchoRoom.Application.Features.Authentication.RefreshToken;

public sealed class RefreshTokenResponse
{
    public TokensDto Tokens { get; set; } = null!;
    public bool StaySignIn { get; set; }
}