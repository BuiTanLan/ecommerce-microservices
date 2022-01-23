using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;
using Catalog.Suppliers;

namespace Catalog.Products.Features.ChangingProductSupplier;

public record ChangingProductSupplierDomainEvent(long SupplierId) : DomainEvent;

internal class ValidateSupplierExistsDomainEventHandler :
    IDomainEventHandler<ChangingProductSupplierDomainEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ValidateSupplierExistsDomainEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(ChangingProductSupplierDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.SupplierId, nameof(notification.SupplierId));
        Guard.Against.SupplierNotFound(
            await _catalogDbContext.SupplierExistsAsync(notification.SupplierId, cancellationToken: cancellationToken),
            notification.SupplierId);
    }
}
