namespace EchoRoom.Database.Infrastructure.Repositories.Implementations;

internal sealed class UserRepository(EchoRoomContext context) : IUserRepository
{
    public EchoRoomContext Context { get; } = context;

    public async Task<User?> GetUserWithRoleByStreamerNickname(string streamerNickname)
        => await Context.Set<User>()
            .AsTracking()
            .Where(user => user.StreamerNickname == streamerNickname)
            .Include(user => user.Role)
            .FirstOrDefaultAsync();

    public async Task<(ICollection<TResult> Users, int TotalCount)> GetManagingUsersAsync<TResult>(
        Expression<Func<User, TResult>> selectExpression, 
        UserManagingQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query = Context.Users.AsNoTracking();

        if (queryParams.RoleIds.Count > 0)
            query = query.Where(user => queryParams.RoleIds.Contains(user.RoleId));

        if (queryParams.CountryIds.Count > 0)
            query = query.Where(user => queryParams.CountryIds.Contains(user.CountryId));

        if (!string.IsNullOrWhiteSpace(queryParams.SearchQuery))
            query = query.Where(user => user.StreamerNickname.Contains(queryParams.SearchQuery, StringComparison.CurrentCultureIgnoreCase));

        query = query.ApplySorting(queryParams.SortProperty, queryParams.SortDirection);

        int totalCount = await query.CountAsync(cancellationToken);

        List<TResult> users = await query
            .Skip(queryParams.Skip)
            .Take(queryParams.Take)
            .Select(selectExpression)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }
}