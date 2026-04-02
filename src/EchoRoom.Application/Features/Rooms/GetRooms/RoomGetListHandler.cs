namespace EchoRoom.Application.Features.Rooms.GetRooms;

public sealed class RoomGetListHandler(
    IUnitOfWork unitOfWork, 
    ILogger<RoomGetListHandler> logger) : IRequestHandler<RoomGetQuery, ApiResult<RoomGetListResponse>>
{
    public async Task<ApiResult<RoomGetListResponse>> Handle(RoomGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            (ICollection<RoomGetResponse> rooms, int totalCount) = await unitOfWork.RoomRepository.GetRoomsAsync(
                MapToRoomGetResponse, 
                request.SearchWord, 
                request.SkipItems,
                request.TakeItems,
                request.Sorting.PropertyName, 
                request.Sorting.Direction, 
                cancellationToken);

            RoomGetListResponse roomGetListResponse = new(rooms, totalCount);

            return ApiResult<RoomGetListResponse>.Success(StatusCodes.Status200OK, roomGetListResponse);
        }
        catch (Exception exception) 
        {
            logger.LogError(exception, "An error occurred while getting the list of rooms.");

            return ApiResult<RoomGetListResponse>.Fail(
                 StatusCodes.Status500InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private static readonly Expression<Func<Room, RoomGetResponse>> MapToRoomGetResponse = room => new RoomGetResponse(
        room.Id, 
        room.Title, 
        room.Creator.StreamerNickname, 
        room.Creator.FullName,
        room.CreatedAt,
        room.TokensForJoin, 
        room.MaxDurationMinutes,
        room.CurrentConnections);
}