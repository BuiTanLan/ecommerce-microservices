using ECommerce.Services.Catalogs.Shared.Data;

namespace ECommerce.Services.Catalogs.Shared.Extensions.ApplicationBuilderExtensions;

using BuildingBlocks.Messaging.Outbox.EF;
using BuildingBlocks.Scheduling.Internal;

public static partial class ApplicationBuilderExtensions
{
    public static async Task ApplyDatabaseMigrations(this IApplicationBuilder app, ILogger logger)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        if (configuration.GetValue<bool>("UseInMemoryDatabase") == false)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var catalogDbContext = serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            var internalMessagesDbContext = serviceScope.ServiceProvider.GetRequiredService<InternalMessageDbContext>();
            var outboxDbContext = serviceScope.ServiceProvider.GetRequiredService<OutboxDataContext>();

            logger.LogInformation("Updating catalog database...");

            await internalMessagesDbContext.Database.MigrateAsync();
            await outboxDbContext.Database.MigrateAsync();
            await catalogDbContext.Database.MigrateAsync();

            logger.LogInformation("Updated catalog database");
        }
    }
}
