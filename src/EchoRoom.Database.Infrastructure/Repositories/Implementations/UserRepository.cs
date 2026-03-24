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
}