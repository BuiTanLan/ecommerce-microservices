using Ardalis.GuardClauses;
using ECommerce.Services.Catalogs.Shared.Contracts;
using ECommerce.Services.Catalogs.Shared.Extensions;
using ECommerce.Services.Catalogs.Suppliers;
using MicroBootstrap.Abstractions.Core.Domain.Events.Internal;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductSupplier.Events;

public record ChangingProductSupplier(SupplierId SupplierId) : DomainEvent;

internal class ChangingSupplierValidationHandler :
    IDomainEventHandler<ChangingProductSupplier>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ChangingSupplierValidationHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(ChangingProductSupplier notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.SupplierId, nameof(notification.SupplierId));
        Guard.Against.ExistsSupplier(
            await _catalogDbContext.SupplierExistsAsync(notification.SupplierId, cancellationToken: cancellationToken),
            notification.SupplierId);
    }
}
