using MicroBootstrap.Messaging;
using MicroBootstrap.Messaging.Postgres.Extensions;
using MicroBootstrap.Monitoring;
using MicroBootstrap.Scheduling.Internal.Extensions;

namespace ECommerce.Services.Catalogs.Shared.Extensions.ApplicationBuilderExtensions;

public static partial class ApplicationBuilderExtensions
{
    public async static Task<IApplicationBuilder> UseInfrastructure(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        ILogger logger)
    {
        app.UseMonitoring();
        await app.UsePostgresMessaging(logger);
        await app.UseInternalScheduler(logger);

        return app;
    }
}