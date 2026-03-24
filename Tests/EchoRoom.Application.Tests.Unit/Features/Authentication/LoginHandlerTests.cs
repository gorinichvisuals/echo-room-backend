namespace EchoRoom.Application.Tests.Unit.Features.Authentication;

public sealed class LoginHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IJwtService _jwtServiceMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ILogger<LoginHandler> _loggerMock;

    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _cacheServiceMock = Substitute.For<ICacheService>();
        _loggerMock = Substitute.For<ILogger<LoginHandler>>();

        _handler = new LoginHandler(
            _unitOfWorkMock, 
            _jwtServiceMock, 
            _cacheServiceMock, 
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserWithRequestedEmailDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        User? existingUser = null;

        LoginCommand request = new()
        {
            StreamerNickname = "testStreamer",
            Password = "testPassword"
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname)
            .Returns(existingUser);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<LoginResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Unauthorized);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_DOES_NOT_EXIST_NICKNAME));
        result.ErrorMessage.ShouldBe("User with the current nickname does not exist.");
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenUserTemporaryLocked()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        User existingUser = new() 
        {
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = "mintest",
            LockoutEnd = DateTime.UtcNow.AddMinutes(10)
        };

        LoginCommand request = new()
        {
            StreamerNickname = "testStreamer",
            Password = "testPassword"
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname)
            .Returns(existingUser);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<LoginResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Forbidden);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_LOCKED_OUT));
        result.ErrorMessage.ShouldBe("Account is temporarily locked. Try again in 15 minutes.");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenPasswordIncorrect()
    {
        // Arrange
        User user = new()
        {
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = Argon2.Hash("correctPassword"),
            FailedLoginAttempts = 4,
        };

        LoginCommand request = new() 
        {
            StreamerNickname = "testStreamer",
            Password = "wrongPassword",
            StaySignIn = false
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname)
            .Returns(user);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.BadRequest);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_INVALID_PASSWORD));
        user.FailedLoginAttempts.ShouldBe(0);
        user.LockoutEnd.HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenPasswordCorrect_AndStaySignInTrue()
    {
        // Arrange
        User user = new() 
        {
            Id = 1,
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = Argon2.Hash("correctPassword"),
            Role = new Role 
            { 
                Id = 1, 
                Name = "Admin" 
            }
        };

        LoginCommand request = new()
        {
            StreamerNickname = "testStreamer",
            Password = "correctPassword",
            StaySignIn = true
        };

        string accessToken = "access-token";
        string refreshToken = "refresh-token";

        LoginResponse expectedResult = new()
        {
            Tokens = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            },
            StaySignIn = request.StaySignIn,
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname)
            .Returns(user);

        _jwtServiceMock.CreateAccessToken(user.Id, user.Email, user.Role.Name, user.StreamerNickname)
            .Returns(accessToken);

        _jwtServiceMock.CreateRefreshToken(user.Id, user.Email)
            .Returns(refreshToken);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data!.ShouldBeEquivalentTo(expectedResult);

        await _cacheServiceMock.Received(1)
            .Set(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>());
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenPasswordCorrect_AndStaySignInFalse()
    {
        // Arrange
        User user = new()
        {
            Id = 1,
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = Argon2.Hash("correctPassword"),
            Role = new Role
            {
                Id = 1,
                Name = "Admin"
            }
        };

        LoginCommand request = new()
        {
            StreamerNickname = "testStreamer",
            Password = "correctPassword",
            StaySignIn = false
        };

        string accessToken = "access-token";

        LoginResponse expectedResult = new()
        {
            Tokens = new()
            {
                AccessToken = accessToken,
                RefreshToken = string.Empty
            },
            StaySignIn = request.StaySignIn,
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname)
            .Returns(user);

        _jwtServiceMock.CreateAccessToken(user.Id, user.Email, user.Role.Name, user.StreamerNickname)
            .Returns(accessToken);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data!.ShouldBeEquivalentTo(expectedResult);

        await _cacheServiceMock.DidNotReceive()
            .Set(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>());
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenUserRepositoryReturnsDbError()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        LoginCommand request = new()
        {
            StreamerNickname = "testStreamer",
            Password = "testPassword"
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByStreamerNickname(request.StreamerNickname)
            .Throws(new Exception("Error occurred while login user."));

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<LoginResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while login user.");
    }
}