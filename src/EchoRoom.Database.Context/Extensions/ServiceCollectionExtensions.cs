namespace EchoRoom.Database.Context.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EchoRoomContext>(options =>
            options.UseNpgsql(connectionString)
                   .EnableSensitiveDataLogging()
                   .LogTo(Console.WriteLine, LogLevel.Information));
    }
}