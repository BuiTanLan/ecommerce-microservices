using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Core.Contracts;
using Catalog.Infrastructure.Extensions;
using Catalog.Suppliers;

namespace Catalog.Products.Features.CreateProduct.Events.Domain.Handlers;

internal class ValidateSupplierExistsDomainEventHandler :
    IDomainEventHandler<CreatingProductDomainEvent>,
    IDomainEventHandler<ChangingProductSupplierDomainEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ValidateSupplierExistsDomainEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(CreatingProductDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.SupplierId, nameof(notification.SupplierId));
        Guard.Against.SupplierNotFound(
            await _catalogDbContext.SupplierExistsAsync(notification.Id, cancellationToken: cancellationToken),
            notification.Id);
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
