namespace EchoRoom.Database.Infrastructure.UoW;

public interface IUnitOfWork
{
    ICountryRepository CountryRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserRepository UserRepository { get; }

    Task Save();
}