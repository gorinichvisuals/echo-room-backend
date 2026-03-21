namespace EchoRoom.Application.Features.Authentication.Login;

public sealed class LoginHandler(
    IUnitOfWork unitOfWork, 
    IJwtService jwtService, 
    ILogger<LoginHandler> logger) : IRequestHandler<LoginCommand, ApiResult<LoginResponse>>
{
    public async Task<ApiResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await unitOfWork.UserRepository.GetUserWithRoleByEmail(request.Email);

            if (existingUser is null)
                return ApiResult<LoginResponse>.Fail(
                    EchoRoomHttpStatusCode.Unauthorized, "User with the current email does not exist.", ErrorStatusCode.USER_DOES_NOT_EXIST_EMAIL);

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

    private LoginResponse CreateResponse(User userDto, bool staySignIn)
        => new()
        {
            Tokens = new TokensDto
            {
                AccessToken = jwtService.CreateAccessToken(userDto.Id, userDto.Email, userDto.Role.Name, userDto.StreamerNickname),
                RefreshToken = staySignIn
                    ? jwtService.CreateRefreshToken(userDto.Id, userDto.Email)
                    : string.Empty,
            },
            StaySignIn = staySignIn
        };
}