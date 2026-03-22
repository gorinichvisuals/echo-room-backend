
namespace EchoRoom.Application.Tests.Unit.Features.Users;

public sealed class UserGetPersonalInfoHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<UserGetPersonalInfoHandler> _loggerMock;

    private readonly UserGetPersonalInfoHandler _handler;

    public UserGetPersonalInfoHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<UserGetPersonalInfoHandler>>();

        _handler = new UserGetPersonalInfoHandler(_unitOfWorkMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserGetPersonalInfoCommand command = new() 
        { 
            UserId = 1 
        };

        UserGetPersonalInfoResponse userPersonalInfo = new()
        {
            Id = 1,
            Email = "test@gmail.com",
            StreamerNickname = "Streamer",
            FullName = "Full name",
            Phone = "+380500000000",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc)
        };

        _unitOfWorkMock.UserRepository
            .GetMappedItem(
                Arg.Any<Expression<Func<User, UserGetPersonalInfoResponse>>>(), 
                Arg.Any<Expression<Func<User, bool>>>(), 
                cancellationToken)
            .Returns(userPersonalInfo);

        // Act
        ApiResult<UserGetPersonalInfoResponse> result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult< UserGetPersonalInfoResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.Data.ShouldBeEquivalentTo(userPersonalInfo);
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_whenUserNotExists()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserGetPersonalInfoCommand command = new()
        {
            UserId = 1
        };

        UserGetPersonalInfoResponse? userPersonalInfo = null;

        _unitOfWorkMock.UserRepository
            .GetMappedItem(
                Arg.Any<Expression<Func<User, UserGetPersonalInfoResponse>>>(),
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken)
            .Returns(userPersonalInfo);

        // Act
        ApiResult<UserGetPersonalInfoResponse> result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserGetPersonalInfoResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_DATA_NOT_FOUND));
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.NotFound);
        result.ErrorMessage.ShouldBe("User data not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenDbIssue()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserGetPersonalInfoCommand command = new()
        {
            UserId = 1
        };

        _unitOfWorkMock.UserRepository
            .GetMappedItem(
                Arg.Any<Expression<Func<User, UserGetPersonalInfoResponse>>>(),
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken)
            .Throws(new Exception("Error occurred while retrieving user data."));

        // Act
        ApiResult<UserGetPersonalInfoResponse> result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserGetPersonalInfoResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorMessage.ShouldBe("Error occurred while retrieving user data.");
    }
}