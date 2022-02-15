using ECommerce.Services.Identity.Shared.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Services.Identity.Shared.Extensions.ApplicationBuilderExtensions;

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
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<IdentityContext>();
            var internalMessagesDbContext = serviceScope.ServiceProvider.GetRequiredService<InternalMessageDbContext>();
            var outboxDbContext = serviceScope.ServiceProvider.GetRequiredService<OutboxDataContext>();

            logger.LogInformation("Updating database...");

            await internalMessagesDbContext.Database.MigrateAsync();
            await outboxDbContext.Database.MigrateAsync();
            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Updated database");
        }
    }
}
