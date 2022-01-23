using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Extensions;
using MediatR;

namespace BuildingBlocks.Core.Domain.Events;

public class EventProcessor : IEventProcessor
{
    private readonly IMediator _mediator;

    public EventProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            await _mediator.DispatchIntegrationEventAsync(integrationEvent, cancellationToken: cancellationToken);
            return;
        }

        if (@event is IDomainEvent domainEvent)
        {
            await _mediator.DispatchDomainEventAsync(domainEvent, cancellationToken: cancellationToken);
            return;
        }

        if (@event is IDomainNotificationEvent notificationEvent)
        {
            await _mediator.DispatchDomainNotificationEventAsync(notificationEvent, cancellationToken: cancellationToken);
            return;
        }

        await _mediator.Publish(@event, cancellationToken);
    }

    public async Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }
}
