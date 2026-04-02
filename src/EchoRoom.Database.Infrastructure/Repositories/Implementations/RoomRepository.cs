namespace EchoRoom.Database.Infrastructure.Repositories.Implementations;

internal sealed class RoomRepository(EchoRoomContext context) : IRoomRepository
{
    public EchoRoomContext Context { get; } = context;

    public async Task<(ICollection<TResult> Rooms, int TotalCount)> GetRoomsAsync<TResult>(
        Expression<Func<Room, TResult>> selectExpression, 
        string? searchWord, 
        int skipItems,
        int takeItems,
        RoomSortProperty sortProperty,
        SortDirection direction,
        CancellationToken cancellationToken)
    {
        IQueryable<Room> query = Context.Rooms.AsNoTracking().Where(room => room.IsActive);

        if (!string.IsNullOrWhiteSpace(searchWord))
            query = query.Where(room => room.Creator.StreamerNickname.Contains(searchWord, StringComparison.CurrentCultureIgnoreCase));

        query = query.ApplySorting(sortProperty, direction);

        int totalCount = await query.CountAsync(cancellationToken);

        ICollection<TResult> rooms = await query
            .Skip(skipItems)
            .Take(takeItems)
            .Select(selectExpression)
            .ToListAsync(cancellationToken);

        return (rooms, totalCount);
    }
}