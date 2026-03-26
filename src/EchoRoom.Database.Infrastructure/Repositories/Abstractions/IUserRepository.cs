namespace EchoRoom.Database.Infrastructure.Repositories.Abstractions;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserWithRoleByStreamerNickname(string streamerNickname);
    Task<(ICollection<TResult> Users, int TotalCount)> GetManagingUsersAsync<TResult>(
        Expression<Func<User, TResult>> selectExpression,
        UserManagingQueryParams queryParams,
        CancellationToken cancellationToken);
}