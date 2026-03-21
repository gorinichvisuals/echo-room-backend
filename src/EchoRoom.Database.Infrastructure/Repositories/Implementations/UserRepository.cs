namespace EchoRoom.Database.Infrastructure.Repositories.Implementations;

internal sealed class UserRepository(EchoRoomContext context) : IUserRepository
{
    public EchoRoomContext Context { get; } = context;

    public async Task<User?> GetUserWithRoleByEmail(string email)
        => await Context.Set<User>()
            .AsTracking()
            .Where(user => user.Email == email)
            .Include(user => user.Role)
            .FirstOrDefaultAsync();
}