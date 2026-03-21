namespace EchoRoom.Database.Infrastructure.Repositories.Implementations;

internal sealed class CountryRepository(EchoRoomContext context) : ICountryRepository
{
    public EchoRoomContext Context { get; } = context;
}