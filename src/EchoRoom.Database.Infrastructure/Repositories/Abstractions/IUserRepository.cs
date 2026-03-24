namespace EchoRoom.Database.Infrastructure.Repositories.Abstractions;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserWithRoleByStreamerNickname(string streamerNickname);
}