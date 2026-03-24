namespace EchoRoom.Application.Features.Authentication.RefreshToken;

public sealed class RefreshTokenHandler(
    IUnitOfWork unitOfWork, 
    IJwtService jwtService,
    ICacheService cacheService,
    ILogger<RefreshTokenHandler> logger) : IRequestHandler<RefreshTokenCommand, ApiResult<RefreshTokenResponse>>
{
    public async Task<ApiResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            string key = EchoRoomCache.RefreshTokenKey + request.Email;

            (bool tokenExists, string? existingRefreshToken) = await cacheService.TryGet<string>(key);

            if (!tokenExists || existingRefreshToken != request.RefreshToken)
                return ApiResult<RefreshTokenResponse>.Fail(
                    EchoRoomHttpStatusCode.Unauthorized, "Invalid or expired refresh token", ErrorStatusCode.INVALID_REFRESH_TOKEN);

            User? existingUser = await unitOfWork.UserRepository
                .GetItemWIthIncludes(user => user.Email == request.Email, cancellationToken, user => user.Role);

            if (existingUser is null)
                return ApiResult<RefreshTokenResponse>.Fail(
                    EchoRoomHttpStatusCode.Unauthorized, "User not found, cannot refresh token", ErrorStatusCode.USER_DATA_NOT_FOUND);

            RefreshTokenResponse refreshTokenResponse = CreateResponse(existingUser);

            existingUser.LastLoginAt = DateTime.UtcNow;
            await unitOfWork.Save();

            return ApiResult<RefreshTokenResponse>.Success(EchoRoomHttpStatusCode.OK, refreshTokenResponse);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while refreshing token for user.");

            return ApiResult<RefreshTokenResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private RefreshTokenResponse CreateResponse(User user)
    {
        RefreshTokenResponse refreshTokenResponse = new()
        {
            Tokens = new TokensDto
            {
                AccessToken = jwtService.CreateAccessToken(user.Id, user.Email, user.Role.Name, user.StreamerNickname),
                RefreshToken = jwtService.CreateRefreshToken(user.Id, user.Email)
            },
            StaySignIn = true
        };

        string refreshTokenKey = EchoRoomCache.RefreshTokenKey + user.Email;
        cacheService.Set(refreshTokenKey, refreshTokenResponse.Tokens.RefreshToken, TimeSpan.FromDays(2));

        return refreshTokenResponse;
    }
}