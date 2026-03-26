namespace EchoRoom.Application.Tests.Unit.Features.Users;

public sealed class UserGetManagingListHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<UserGetManagingListHandler> _loggerMock;

    private readonly UserGetManagingListHandler _handler;

    public UserGetManagingListHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<UserGetManagingListHandler>>();

        _handler = new UserGetManagingListHandler(
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenUsersExist()
    {
        // Arrange
        UserGetManagingListQuery query = new()
        {
            SearchQuery = "test",
            SkipItems = 0,
            TakeItems = 10,
            Sorting = new SortingDto
            {
                PropertyName = UserSortProperty.Username,
                Direction = Shared.Constants.Enums.SortDirection.ASCENDING
            },
            Filters = new UserGetManagingFilters
            {
                RoleIds = new List<int> { 1 },
                CountryIds = new List<int> { 2 }
            }
        };

        ICollection<UserGetManagingResponse> users =
        [
            new()
            {
                Id = 1,
                RoleId = 1,
                CountryId = 2,
                FullName = "Full Name",
                StreamerNickname = "Streamer",
                Email = "user@test.com",
                PhoneNumber = "+380500000000",
                BirthDate = DateTime.UtcNow.AddYears(-20),
                IsAdmin = false,
                IsStreamer = true,
                CanBeEditedByOtherAdmin = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                LastLoginAt = DateTime.UtcNow
            }
        ];

        _unitOfWorkMock.UserRepository
            .GetManagingUsersAsync(
                Arg.Any<Expression<Func<User, UserGetManagingResponse>>>(), 
                Arg.Any<UserManagingQueryParams>(), 
                Arg.Any<CancellationToken>())
            .Returns((users, users.Count));

        // Act
        ApiResult<UserGetManagingListResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<UserGetManagingListResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Users.ShouldBe(users);
        result.Data.TotalCount.ShouldBe(users.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersFound()
    {
        // Arrange
        UserGetManagingListQuery query = new();

        _unitOfWorkMock.UserRepository
            .GetManagingUsersAsync(
                Arg.Any<Expression<Func<User, UserGetManagingResponse>>>(), 
                Arg.Any<UserManagingQueryParams>(), 
                Arg.Any<CancellationToken>())
            .Returns((new List<UserGetManagingResponse>(), 0));

        // Act
        ApiResult<UserGetManagingListResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<UserGetManagingListResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Users.ShouldBeEmpty();
        result.Data.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        UserGetManagingListQuery query = new();

        _unitOfWorkMock.UserRepository
            .GetManagingUsersAsync(
                Arg.Any<Expression<Func<User, UserGetManagingResponse>>>(), 
                Arg.Any<UserManagingQueryParams>(), 
                Arg.Any<CancellationToken>())
            .Throws(new Exception("Error occurred while retrieving users data."));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<UserGetManagingListResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while retrieving users data.");
    }
}