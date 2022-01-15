using Ardalis.GuardClauses;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.EFCore;
using BuildingBlocks.Messaging.Outbox.InMemory;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Outbox;

public class TxOutboxConstants
{
    public const string InMemory = "inmem";
    public const string EntityFramework = "ef";
}

public static class Extensions
{
    public static IServiceCollection AddTransactionalOutbox(
        this IServiceCollection services,
        IConfiguration configuration,
        string provider = TxOutboxConstants.InMemory)
    {
        switch (provider)
        {
            case TxOutboxConstants.InMemory:
            {
                services.AddSingleton<IEventStorage, EventStorage>();
                services.AddScoped<INotificationHandler<EventWrapper>, InMemoryOutboxHandler>();
                services.AddScoped<ITxOutboxProcessor, InMemoryTxOutboxProcessor>();
                break;
            }

            case TxOutboxConstants.EntityFramework:
            {
                var outboxOption = Guard.Against.Null(configuration.GetEfOutboxOptions(), nameof(EfOutboxOptions));

                services.AddDbContext<OutboxMessageDataContext>(options =>
                {
                    options.UseNpgsql(outboxOption.ConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(OutboxMessageDataContext).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }).UseSnakeCaseNamingConvention();
                });

                services.AddUnitOfWork<OutboxMessageDataContext>();
                services.AddScoped<INotificationHandler<EventWrapper>, EfOutboxHandler>();
                services.AddScoped<ITxOutboxProcessor, EfTxOutboxProcessor>();
                break;
            }
        }

        return services;
    }
}
