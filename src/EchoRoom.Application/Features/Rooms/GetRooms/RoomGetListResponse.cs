namespace EchoRoom.Application.Features.Rooms.GetRooms;

public sealed record RoomGetListResponse(
    ICollection<RoomGetResponse> Rooms, 
    int TotalCount);