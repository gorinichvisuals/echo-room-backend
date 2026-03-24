namespace EchoRoom.Application.Features.Authentication.RefreshToken;

public sealed class RefreshTokenCommand : IRequest<ApiResult<RefreshTokenResponse>>
{
    public required string Email { get; set; }
    public required string RefreshToken { get; set; }
}