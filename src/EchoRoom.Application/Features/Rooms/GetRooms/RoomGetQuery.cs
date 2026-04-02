namespace EchoRoom.Application.Features.Rooms.GetRooms;

public sealed class RoomGetQuery : IRequest<ApiResult<RoomGetListResponse>>
{
    public string SearchWord { get; set; } = string.Empty;
    public int SkipItems { get; set; } = 0;
    public int TakeItems { get; set; } = 20;
    public SortingDto SortingDto { get; set; } = new SortingDto();
}

public sealed class SortingDto
{
    public RoomSortProperty PropertyName { get; set; } = RoomSortProperty.CreatedAt;
    public SortDirection Direction { get; set; } = SortDirection.DESCENDING;   
}