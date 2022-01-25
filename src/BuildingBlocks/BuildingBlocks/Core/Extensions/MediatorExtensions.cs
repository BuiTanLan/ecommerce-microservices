using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Messaging.Serialization;
using MediatR;
using Serilog;

namespace BuildingBlocks.Core.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventAsync(
        this IMediator mediator,
        IList<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(domainEvents, nameof(domainEvents));

        var tasks = domainEvents
            .Select(async domainEvent =>
            {
                await DispatchDomainEventAsync(mediator, domainEvent, cancellationToken);
            });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public static async Task DispatchDomainEventAsync(
        this IMediator mediator,
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(domainEvent, nameof(domainEvent));
        var serializer = ServiceActivator.GetRequiredService<IMessageSerializer>();

        await mediator.Publish(domainEvent, cancellationToken);
        Log.Logger.Debug(
            "Published domain event {DomainEventName} with payload {DomainEventContent}",
            domainEvent.GetType().FullName,
            serializer.Serialize(domainEvent));
    }

    public static async Task DispatchDomainNotificationEventAsync(
        this IMediator mediator,
        IList<IDomainNotificationEvent> domainNotificationEvents,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(domainNotificationEvents, nameof(domainNotificationEvents));

        var tasks = domainNotificationEvents
            .Select(async domainNotificationEvent =>
            {
                await DispatchDomainNotificationEventAsync(mediator, domainNotificationEvent, cancellationToken);
            });
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public static async Task DispatchDomainNotificationEventAsync(
        this IMediator mediator,
        IDomainNotificationEvent domainNotificationEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(domainNotificationEvent, nameof(domainNotificationEvent));
        var serializer = ServiceActivator.GetRequiredService<IMessageSerializer>();

        await mediator.Publish(domainNotificationEvent, cancellationToken);
        Log.Logger.Debug(
            "Published domain notification event {DomainNotificationEventName} with payload {DomainNotificationEventContent}",
            domainNotificationEvent.GetType().FullName,
            serializer.Serialize(domainNotificationEvent));
    }

    public static async Task DispatchIntegrationEventAsync(
        this IMediator mediator,
        IList<IIntegrationEvent> integrationEvents,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        var tasks = integrationEvents
            .Select(async integrationEvent =>
            {
                await DispatchIntegrationEventAsync(mediator, integrationEvent, cancellationToken);
            });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public static async Task DispatchIntegrationEventAsync(
        this IMediator mediator,
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        var serializer = ServiceActivator.GetRequiredService<IMessageSerializer>();

        await mediator.Publish(integrationEvent, cancellationToken);
        Log.Logger.Debug(
            "Published integration notification event {IntegrationEventName} with payload {IntegrationEventContent}",
            integrationEvent.GetType().FullName,
            serializer.Serialize(integrationEvent));
    }
}
