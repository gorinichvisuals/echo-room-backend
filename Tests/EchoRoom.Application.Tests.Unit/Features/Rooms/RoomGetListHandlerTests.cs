namespace EchoRoom.Application.Tests.Unit.Features.Rooms;

public sealed class RoomGetListHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<RoomGetListHandler> _loggerMock;

    private readonly RoomGetListHandler _handler;

    public RoomGetListHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<RoomGetListHandler>>();

        _handler = new RoomGetListHandler(
            _unitOfWorkMock,
            _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenRoomsExist()
    {
        // Arrange
        RoomGetQuery query = new()
        {
            SearchWord = "test",
            SkipItems = 0,
            TakeItems = 10,
            Sorting = new Application.Features.Rooms.GetRooms.SortingDto
            {
                PropertyName = RoomSortProperty.CreatedAt,
                Direction = Shared.Constants.Enums.SortDirection.DESCENDING
            }
        };

        ICollection<RoomGetResponse> rooms =
        [
            new RoomGetResponse(
                Guid.NewGuid(),
                "Room 1",
                "Streamer1",
                "Full Name 1",
                DateTime.UtcNow,
                100,
                60,
                5)
        ];

        _unitOfWorkMock.RoomRepository
            .GetRoomsAsync(
                Arg.Any<Expression<Func<Room, RoomGetResponse>>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<RoomSortProperty>(),
                Arg.Any<Shared.Constants.Enums.SortDirection>(),
                Arg.Any<CancellationToken>())
            .Returns((rooms, rooms.Count));

        // Act
        ApiResult<RoomGetListResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RoomGetListResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);

        result.Data.ShouldNotBeNull();
        result.Data.Rooms.ShouldBe(rooms);
        result.Data.TotalCount.ShouldBe(rooms.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoRoomsFound()
    {
        // Arrange
        RoomGetQuery query = new();

        _unitOfWorkMock.RoomRepository
            .GetRoomsAsync(
                Arg.Any<Expression<Func<Room, RoomGetResponse>>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<RoomSortProperty>(),
                Arg.Any<Shared.Constants.Enums.SortDirection>(),
                Arg.Any<CancellationToken>())
            .Returns((new List<RoomGetResponse>(), 0));

        // Act
        ApiResult<RoomGetListResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RoomGetListResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);

        result.Data.ShouldNotBeNull();
        result.Data.Rooms.ShouldBeEmpty();
        result.Data.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        RoomGetQuery query = new();

        _unitOfWorkMock.RoomRepository
            .GetRoomsAsync(
                Arg.Any<Expression<Func<Room, RoomGetResponse>>>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<RoomSortProperty>(),
                Arg.Any<Shared.Constants.Enums.SortDirection>(),
                Arg.Any<CancellationToken>())
            .Throws(new Exception("Error occurred while retrieving rooms data."));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ApiResult<RoomGetListResponse>>();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while retrieving rooms data.");
    }
}