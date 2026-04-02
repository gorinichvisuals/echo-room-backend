namespace EchoRoom.Database.Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static void AddDatabaseInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();

        services.AddDatabaseContext(connectionString);
    }
}