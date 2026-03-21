namespace EchoRoom.Database.Infrastructure.Repositories.Implementations;

internal sealed class RoleRepository(EchoRoomContext context) : IRoleRepository
{
    public EchoRoomContext Context { get; } = context;
}