namespace EchoRoom.Application.Features.Authentication.Login;

public sealed class LoginCommand : IRequest<ApiResult<LoginResponse>>
{
    [Required, MaxLength(255)]
    public required string StreamerNickname { get; set; }

    [Required, MinLength(6)]
    public required string Password { get; set; }

    public bool StaySignIn { get; set; }
}