using Ardalis.GuardClauses;
using BuildingBlocks.EFCore;
using BuildingBlocks.Messaging.Scheduling;
using BuildingBlocks.Scheduling.Internal.MessagesScheduler;
using BuildingBlocks.Scheduling.Internal.Services;
using BuildingBlocks.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Scheduling.Internal;

public static class Extensions
{
    public static IServiceCollection AddInternalScheduler<TContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : AppDbContextBase
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddOptions<InternalMessageSchedulerOptions>().Bind(configuration.GetSection(nameof(InternalMessageSchedulerOptions)))
            .ValidateDataAnnotations();

        services.AddDbContext<TContext>(cfg =>
        {
            var options = Guard.Against.Null(
                configuration.GetOptions<InternalMessageSchedulerOptions>(nameof(InternalMessageSchedulerOptions)),
                nameof(InternalMessageSchedulerOptions));

            cfg.UseNpgsql(options.ConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IMessagesScheduler, InternalMessageScheduler>();
        services.AddScoped<IInternalMessageService, InternalMessageService>();

        services.AddHostedService<InternalMessageSchedulerBackgroundWorkerService>();

        return services;
    }
}
