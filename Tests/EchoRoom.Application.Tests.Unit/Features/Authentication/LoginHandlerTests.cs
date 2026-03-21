using Isopoh.Cryptography.Argon2;

namespace EchoRoom.Application.Tests.Unit.Features.Authentication;

public sealed class LoginHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IJwtService _jwtServiceMock;
    private readonly ILogger<LoginHandler> _loggerMock;

    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _loggerMock = Substitute.For<ILogger<LoginHandler>>();

        _handler = new LoginHandler(_unitOfWorkMock, _jwtServiceMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserWithRequestedEmailDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        User? existingUser = null;

        LoginCommand request = new()
        {
            Email = "test@gmail.com",
            Password = "testPassword"
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByEmail(request.Email)
            .Returns(existingUser);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<LoginResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Unauthorized);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_DOES_NOT_EXIST_EMAIL));
        result.ErrorMessage.ShouldBe("User with the current email does not exist.");
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
            Email = "test@gmail.com",
            Password = "testPassword"
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByEmail(request.Email)
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

        _unitOfWorkMock.UserRepository.GetUserWithRoleByEmail(user.Email)
            .Returns(user);

        LoginCommand request = new() 
        {
            Email = user.Email,
            Password = "wrongPassword",
            StaySignIn = false
        };

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
            Email = user.Email,
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

        _unitOfWorkMock.UserRepository.GetUserWithRoleByEmail(user.Email)
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
            Email = user.Email,
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

        _unitOfWorkMock.UserRepository.GetUserWithRoleByEmail(user.Email)
            .Returns(user);

        _jwtServiceMock.CreateAccessToken(user.Id, user.Email, user.Role.Name, user.StreamerNickname)
            .Returns(accessToken);

        // Act
        ApiResult<LoginResponse> result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data!.ShouldBeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenUserRepositoryReturnsDbError()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        LoginCommand request = new()
        {
            Email = "test@gmail.com",
            Password = "testPassword"
        };

        _unitOfWorkMock.UserRepository.GetUserWithRoleByEmail(request.Email)
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