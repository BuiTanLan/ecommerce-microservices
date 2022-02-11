using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.RestockSubscriptions.Features.UpdatingMultipleMongoRestockSubscriptionReadModel;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Write;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.DeletingRestockSubscriptionsByTime;

public record MultipleRestockSubscriptionDeleted(IList<RestockSubscription> RestockSubscriptions) : DomainEvent;

internal class RestockSubscriptionByTimeDeletedHandler : IDomainEventHandler<MultipleRestockSubscriptionDeleted>
{
    private readonly ICommandProcessor _commandProcessor;

    public RestockSubscriptionByTimeDeletedHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task Handle(MultipleRestockSubscriptionDeleted notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        // https://github.com/kgrzybek/modular-monolith-with-ddd#38-internal-processing
        await _commandProcessor.SendAsync(
            new UpdateMultipleMongoRestockSubscriptionReadModel(notification.RestockSubscriptions), cancellationToken);
    }
}
