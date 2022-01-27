using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalogs.Shared.Core.Contracts;

namespace Catalogs.Products.Features.DebitingProductStock.Events.Domain;

public record ProductRestockThresholdReachedEvent(long ProductId, int Quantity) : DomainEvent;

internal class ProductRestockThresholdReachedEventHandler : IDomainEventHandler<ProductRestockThresholdReachedEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ProductRestockThresholdReachedEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public Task Handle(ProductRestockThresholdReachedEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        // For example send an email to get more products
        return Task.CompletedTask;
    }
}
