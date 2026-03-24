namespace EchoRoom.Application.Features.Authentication.Login;

public sealed class LoginHandler(
    IUnitOfWork unitOfWork, 
    IJwtService jwtService, 
    ICacheService cacheService,
    ILogger<LoginHandler> logger) : IRequestHandler<LoginCommand, ApiResult<LoginResponse>>
{
    public async Task<ApiResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await unitOfWork.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname);

            if (existingUser is null)
                return ApiResult<LoginResponse>.Fail(
                    EchoRoomHttpStatusCode.Unauthorized, "User with the current nickname does not exist.", ErrorStatusCode.USER_DOES_NOT_EXIST_NICKNAME);

            if (existingUser.LockoutEnd.HasValue && existingUser.LockoutEnd.Value > DateTime.UtcNow)
                return ApiResult<LoginResponse>.Fail(
                    EchoRoomHttpStatusCode.Forbidden, "Account is temporarily locked. Try again in 15 minutes.", ErrorStatusCode.USER_LOCKED_OUT);

            bool isPasswordCorrect = Argon2.Verify(existingUser.Password, request.Password);

            if (!isPasswordCorrect)
            {
                existingUser.FailedLoginAttempts++;

                if (existingUser.FailedLoginAttempts >= 5)
                {
                    existingUser.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    existingUser.FailedLoginAttempts = 0;
                }

                await unitOfWork.Save();

                return ApiResult<LoginResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, "Incorrect email or password.", ErrorStatusCode.USER_INVALID_PASSWORD);
            }

            LoginResponse response = CreateResponse(existingUser, request.StaySignIn);

            existingUser.FailedLoginAttempts = 0;
            existingUser.LockoutEnd = null;
            existingUser.LastLoginAt = DateTime.UtcNow;

            await unitOfWork.Save();

            return ApiResult<LoginResponse>.Success(EchoRoomHttpStatusCode.OK, response);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while login user.");

            return ApiResult<LoginResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private LoginResponse CreateResponse(User user, bool staySignIn)
    {
        LoginResponse loginResponse = new()
        {
            Tokens = new TokensDto
            {
                AccessToken = jwtService.CreateAccessToken(user.Id, user.Email, user.Role.Name, user.StreamerNickname),
                RefreshToken = staySignIn
                    ? jwtService.CreateRefreshToken(user.Id, user.Email)
                    : string.Empty,
            },
            StaySignIn = staySignIn
        };

        if (staySignIn)
        {
            string refreshTokenKey = EchoRoomCache.RefreshTokenKey + user.Email;
            cacheService.Set(refreshTokenKey, loginResponse.Tokens.RefreshToken, TimeSpan.FromDays(2));
        }

        return loginResponse;
    }
}