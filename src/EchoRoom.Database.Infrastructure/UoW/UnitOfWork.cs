namespace EchoRoom.Database.Infrastructure.UoW;

internal sealed class UnitOfWork(
    EchoRoomContext context,
    ICountryRepository countryRepository, 
    IRoleRepository roleRepository, 
    IUserRepository userRepository) : IUnitOfWork
{
    public ICountryRepository CountryRepository { get; } = countryRepository;
    public IRoleRepository RoleRepository { get; } = roleRepository;
    public IUserRepository UserRepository { get; } = userRepository;

    public async Task Save() => await context.SaveChangesAsync();
}