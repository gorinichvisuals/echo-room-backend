namespace EchoRoom.Database.Infrastructure.Repositories.Abstractions;

public interface IRoomRepository : IBaseRepository<Room>
{
    Task<(ICollection<TResult> Rooms, int TotalCount)> GetRoomsAsync<TResult>(
        Expression<Func<Room, TResult>> selectExpression,
        string? searchWord,
        int skipItems,
        int takeItems,
        RoomSortProperty sortProperty,
        SortDirection direction,
        CancellationToken cancellationToken);
}