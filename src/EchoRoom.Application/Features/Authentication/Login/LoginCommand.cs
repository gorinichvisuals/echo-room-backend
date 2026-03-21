namespace EchoRoom.Application.Features.Authentication.Login;

public sealed class LoginCommand : IRequest<ApiResult<LoginResponse>>
{
    [Required, EmailAddress, MaxLength(255)]
    public required string Email { get; set; }

    [Required, MinLength(6)]
    public required string Password { get; set; }

    public bool StaySignIn { get; set; }
}