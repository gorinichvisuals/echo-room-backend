namespace EchoRoom.Application.Features.Authentication.Login;

public sealed class LoginResponse
{
    public TokensDto Tokens { get; set; } = null!;
    public bool StaySignIn { get; set; }
}