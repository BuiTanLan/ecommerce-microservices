using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Model;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace BuildingBlocks.EFCore.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(
        this IMediator mediator,
        AppDbContextBase ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        await PublishDomainEventsAsync(mediator, domainEvents);
    }

    public static Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<IDomainEvent> domainEvents)
    {
        return PublishDomainEventsAsync(mediator, domainEvents);
    }

    private static async Task PublishDomainEventsAsync(IMediator mediator, IEnumerable<IDomainEvent> domainEvents)
    {
        var tasks = domainEvents
            .Select(async domainEvent =>
            {
                await mediator.Publish(domainEvent);
                Log.Logger.Debug(
                    "Published domain event {DomainEventName} with payload {DomainEventContent}",
                    domainEvent.GetType().FullName,
                    JsonConvert.SerializeObject(domainEvent));
            });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
