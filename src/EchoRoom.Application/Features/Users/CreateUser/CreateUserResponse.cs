namespace EchoRoom.Application.Features.Users.CreateUser;

public sealed class CreateUserResponse
{
    public TokensDto Tokens { get; set; } = null!;
    public bool StaySignIn { get; set; }
}