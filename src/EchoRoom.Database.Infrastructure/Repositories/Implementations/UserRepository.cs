namespace EchoRoom.Database.Infrastructure.Repositories.Implementations;

internal sealed class UserRepository(EchoRoomContext context) : IUserRepository
{
    public EchoRoomContext Context { get; } = context;
}