namespace EchoRoom.Application.Tests.Unit.Features.Authentication;

public sealed class RefreshTokenHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IJwtService _jwtServiceMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ILogger<RefreshTokenHandler> _loggerMock;

    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _cacheServiceMock = Substitute.For<ICacheService>();
        _loggerMock = Substitute.For<ILogger<RefreshTokenHandler>>();

        _handler = new RefreshTokenHandler(
            _unitOfWorkMock,
            _jwtServiceMock,
            _cacheServiceMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        RefreshTokenCommand request = new()
        {
            Email = "user@test.com",
            RefreshToken = "token"
        };

        _cacheServiceMock.TryGet<string>(Arg.Any<string>())
            .Returns((false, null));

        // Act
        ApiResult<RefreshTokenResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RefreshTokenResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Unauthorized);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INVALID_REFRESH_TOKEN));
        result.ErrorMessage.ShouldBe("Invalid or expired refresh token");
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRefreshTokenDoesNotMatch()
    {
        // Arrange
        RefreshTokenCommand request = new()
        {
            Email = "user@test.com",
            RefreshToken = "token"
        };

        _cacheServiceMock.TryGet<string>(Arg.Any<string>())
            .Returns((true, "other-token"));

        // Act
        ApiResult<RefreshTokenResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RefreshTokenResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Unauthorized);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INVALID_REFRESH_TOKEN));
        result.ErrorMessage.ShouldBe("Invalid or expired refresh token");
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        RefreshTokenCommand request = new()
        {
            Email = "user@test.com",
            RefreshToken = "token"
        };

        User? user = null;

        _cacheServiceMock.TryGet<string>(Arg.Any<string>())
            .Returns((true, "token"));

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<Expression<Func<User, object>>[]>())
            .Returns(user);

        // Act
        ApiResult<RefreshTokenResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RefreshTokenResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Unauthorized);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_DATA_NOT_FOUND));
        result.ErrorMessage.ShouldBe("User not found, cannot refresh token");
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenRefreshTokenValidAndUserExists()
    {
        // Arrange
        RefreshTokenCommand request = new() 
        {
            Email = "user@test.com",
            RefreshToken = "token"
        };

        User user = new()
        {
            Id = 1,
            Email = request.Email,
            FullName = "Full Name",
            PhoneNumber = "+380500000000",
            Password = "hashedPassword",
            StreamerNickname = "Streamer",
            Role = new Role { Id = 1, Name = "BaseUser" },
        };

        RefreshTokenResponse expectedResult = new()
        {
            Tokens = new()
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            },
            StaySignIn = true,
        };

        _cacheServiceMock.TryGet<string>(Arg.Any<string>())
            .Returns((true, "token"));

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(), 
                Arg.Any<CancellationToken>(), 
                Arg.Any<Expression<Func<User, object>>[]>())
            .Returns(user);

        _jwtServiceMock.CreateAccessToken(user.Id, user.Email, user.Role.Name, user.StreamerNickname)
            .Returns("access-token");

        _jwtServiceMock.CreateRefreshToken(user.Id, user.Email)
            .Returns("refresh-token");

        // Act
        ApiResult<RefreshTokenResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RefreshTokenResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data.ShouldBeEquivalentTo(expectedResult);

        await _cacheServiceMock.Received(1)
            .Set(Arg.Any<string>(), "refresh-token", Arg.Any<TimeSpan>());

        user.LastLoginAt.ShouldNotBe(default);
        await _unitOfWorkMock.Received(1)
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        RefreshTokenCommand request = new() 
        {
            Email = "user@test.com",
            RefreshToken = "token"
        };

        _cacheServiceMock.TryGet<string>(Arg.Any<string>())
            .Throws(new Exception("Error occurred while refreshing token for user."));

        // Act
        ApiResult<RefreshTokenResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RefreshTokenResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while refreshing token for user.");
    }
}