using MicroBootstrap.Messaging;
using MicroBootstrap.Messaging.Postgres.Extensions;
using MicroBootstrap.Monitoring;
using MicroBootstrap.Scheduling.Internal.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Services.Identity.Shared.Extensions.ApplicationBuilderExtensions;

public static partial class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> UseInfrastructure(this IApplicationBuilder app, ILogger logger)
    {
        app.UseMonitoring();
        await app.UsePostgresMessaging(logger);
        await app.UseInternalScheduler(logger);

        return app;
    }
}