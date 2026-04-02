namespace EchoRoom.Application.Features.Rooms.GetRooms;

public sealed record RoomGetResponse(
    Guid RoomId,
    string Title,
    string StreamerNickname,
    string FullName,
    DateTime CreatedAt,
    int TokensForJoin,
    int? MaxDurationMinutes,
    int CurrentConnections);