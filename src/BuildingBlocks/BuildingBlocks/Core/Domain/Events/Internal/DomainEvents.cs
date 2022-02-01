using BuildingBlocks.Core.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Domain.Events.Internal;

/// <summary>
/// Execute event handlers immediately
/// Domain Events - Before Persistence
/// Ref https://ardalis.com/immediate-domain-event-salvation-with-mediatr/
/// https://www.weeklydevtips.com/episodes/022
/// </summary>
public static class DomainEvents
{
    private static readonly Func<IMediator> _mediatorFunc =
        ServiceActivator.GetScope().ServiceProvider.GetRequiredService<IMediator>;

    public static async Task RaiseDomainEventAsync(
        IDomainEvent[] domainEvents,
        CancellationToken cancellationToken = default)
    {
        var mediator = _mediatorFunc.Invoke();
        await mediator.DispatchDomainEventAsync(domainEvents, cancellationToken: cancellationToken);
    }

    public static async Task RaiseDomainEventAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var mediator = _mediatorFunc.Invoke();
        await mediator.DispatchDomainEventAsync(domainEvent, cancellationToken: cancellationToken);
    }

    public static void RaiseDomainEvent(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var mediator = _mediatorFunc.Invoke();
        mediator.DispatchDomainEventAsync(domainEvent, cancellationToken: cancellationToken).GetAwaiter().GetResult();
    }
}
