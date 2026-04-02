namespace EchoRoom.Database.Infrastructure.Extensions.Sorting;

public static class RoomSortingExtensions
{
    public static IQueryable<Room> ApplySorting(
        this IQueryable<Room> query,
        RoomSortProperty property,
        SortDirection direction)
    {
        Expression<Func<Room, object>> keySelector = property switch
        {
            RoomSortProperty.StreamerNickname => room => room.Creator.StreamerNickname,
            RoomSortProperty.CurrentConnections => room => room.CurrentConnections,
            RoomSortProperty.TokensForJoin => room => room.TokensForJoin,
            _ => room => room.CreatedAt,
        };

        return direction == SortDirection.ASCENDING
            ? query.OrderBy(keySelector)
            : query.OrderByDescending(keySelector);
    }
}