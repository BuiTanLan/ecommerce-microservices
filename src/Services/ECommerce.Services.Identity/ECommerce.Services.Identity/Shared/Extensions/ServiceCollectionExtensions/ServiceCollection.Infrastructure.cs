using System.Reflection;
using MicroBootstrap.Caching.InMemory;
using MicroBootstrap.Core.Caching;
using MicroBootstrap.Core.Extensions.Configuration;
using MicroBootstrap.Core.Extensions.DependencyInjection;
using MicroBootstrap.Core.Persistence.EfCore;
using MicroBootstrap.CQRS;
using MicroBootstrap.Email;
using MicroBootstrap.Logging;
using MicroBootstrap.Messaging;
using MicroBootstrap.Messaging.Postgres.Extensions;
using MicroBootstrap.Messaging.Transport.Rabbitmq;
using MicroBootstrap.Monitoring;
using MicroBootstrap.Persistence.EfCore.Postgres;
using MicroBootstrap.Scheduling.Internal;
using MicroBootstrap.Scheduling.Internal.Extensions;
using MicroBootstrap.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Services.Identity.Shared.Extensions.ServiceCollectionExtensions;

public static class ServiceCollection
{
    public static WebApplicationBuilder AddInfrastructure(
        this WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        AddInfrastructure(builder.Services, configuration);

        return builder;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCore(configuration);
        services.AddEmailService(configuration);

        services.AddCustomValidators(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddCqrs(doMoreActions: s =>
        {
            s.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>))
                .AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamRequestValidationBehavior<,>))
                .AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamLoggingBehavior<,>))
                .AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamCachingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
        });

        services.AddMonitoring(healthChecksBuilder =>
        {
            var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
            healthChecksBuilder.AddNpgSql(
                postgresOptions.ConnectionString,
                name: "Identity-Postgres-Check",
                tags: new[] { "identity-postgres" });

            var rabbitMqOptions = configuration.GetOptions<RabbitConfiguration>(nameof(RabbitConfiguration));

            healthChecksBuilder.AddRabbitMQ(
                $"amqp://{rabbitMqOptions.UserName}:{rabbitMqOptions.Password}@{rabbitMqOptions.HostName}{rabbitMqOptions.VirtualHost}",
                name: "IdentityService-RabbitMQ-Check",
                tags: new[] { "identity-rabbitmq" });
        });

        services.AddPostgresMessaging(configuration);

        services.AddInternalScheduler(configuration);

        services.AddRabbitMqTransport(configuration);

        services.AddCustomInMemoryCache(configuration)
            .AddCachingRequestPolicies(Assembly.GetExecutingAssembly());

        return services;
    }
}
