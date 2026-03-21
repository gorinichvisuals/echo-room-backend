using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;

using EchoRoom.Database.Context;
using EchoRoom.Database.Context.Extensions;

using EchoRoom.Shared.Constants.Options;

ServiceCollection services = new();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

ConnectionStringOptions connectionStringOptions =
    configuration.GetSection(nameof(ConnectionStringOptions)).Get<ConnectionStringOptions>()!;

services.AddDatabaseContext(connectionStringOptions.EchoRoomConnectionString);

ServiceProvider serviceProvider = services.BuildServiceProvider();

ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Migration started.");

    await using EchoRoomContext context = serviceProvider.GetRequiredService<EchoRoomContext>();

    await context.Database.MigrateAsync();

    logger.LogInformation("Migration completed successfully!");
}
catch (Exception exception)
{
    logger.LogCritical(exception, "An error occurred while migrating the Echo Room Database.");
    throw;
}