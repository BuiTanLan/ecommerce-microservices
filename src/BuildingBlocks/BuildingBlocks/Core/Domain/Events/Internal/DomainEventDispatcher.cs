using System.Collections.Immutable;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Messaging.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Domain.Events.Internal;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IOutboxService _outboxService;
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(
        IOutboxService outboxService,
        IMediator mediator,
        IServiceProvider serviceProvider)
    {
        _outboxService = Guard.Against.Null(outboxService, nameof(outboxService));
        _mediator = Guard.Against.Null(mediator, nameof(mediator));
        _serviceProvider = Guard.Against.Null(serviceProvider, nameof(serviceProvider));
    }

    public async Task DispatchAsync(CancellationToken cancellationToken)
    {
        var domainEventContext = _serviceProvider.GetRequiredService<IDomainEventContext>();

        // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
        // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
        // http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
        // http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
        // https://www.ledjonbehluli.com/posts/domain_to_integration_event/

        // Dispatch our domain events before commit
        var aggregatesTuple = domainEventContext.GetAggregateDomainEvents();
        var events = aggregatesTuple.SelectMany(x => x.DomainEvents).ToList();
        await _mediator.DispatchDomainEventAsync(events, cancellationToken: cancellationToken);

        // Save wrapped integration and notification events to outbox for further processing after commit
        await _outboxService.SaveAsync(
            cancellationToken, events.GetDomainNotificationEventsFromDomainEvents().ToArray());
        await _outboxService.SaveAsync(cancellationToken, events.GetIntegrationEventsFromDomainEvents().ToArray());

        var genericEventMappers = _serviceProvider.GetServices<IEventMapper>().ToList();
        if (genericEventMappers.Any())
        {
            foreach (var genericEventMapper in genericEventMappers)
            {
                genericEventMapper.MapToIntegrationEvents(events);
                genericEventMapper.MapToDomainNotificationEvents(events);
            }
        }

        // Save event mapper events into outbox for further processing after commit
        foreach (var aggregateTuple in aggregatesTuple)
        {
            dynamic? aggregateEventMapper = _serviceProvider.GetService(
                typeof(IEventMapper<>).MakeGenericType(aggregateTuple.Aggregate.GetType()));
            dynamic? integrationEventMapper = _serviceProvider.GetService(
                typeof(IIntegrationEventMapper<>).MakeGenericType(aggregateTuple.Aggregate.GetType()));
            dynamic? notificationMapper = _serviceProvider.GetService(
                typeof(IIDomainNotificationEventMapper<>).MakeGenericType(aggregateTuple.Aggregate.GetType()));

            IEnumerable<IIntegrationEvent>? integrationEvents =
                aggregateEventMapper?.MapToIntegrationEvents(aggregateTuple.DomainEvents.ToImmutableList()) ??
                integrationEventMapper?.MapToIntegrationEvents(aggregateTuple.DomainEvents.ToImmutableList());

            integrationEvents = integrationEvents?.Where(x => x is not null).ToList();
            if (integrationEvents is not null && integrationEvents.Any())
                await _outboxService.SaveAsync(cancellationToken, integrationEvents.ToArray());

            IEnumerable<IDomainNotificationEvent>? notificationEvents =
                aggregateEventMapper?.MapToDomainNotificationEvents(aggregateTuple.DomainEvents.ToImmutableList()) ??
                notificationMapper?.MapToDomainNotificationEvents(aggregateTuple.DomainEvents.ToImmutableList());
            notificationEvents = notificationEvents?.Where(x => x is not null).ToList();

            if (notificationEvents is not null && notificationEvents.Any())
                await _outboxService.SaveAsync(cancellationToken, notificationEvents.ToArray());
        }
    }
}
