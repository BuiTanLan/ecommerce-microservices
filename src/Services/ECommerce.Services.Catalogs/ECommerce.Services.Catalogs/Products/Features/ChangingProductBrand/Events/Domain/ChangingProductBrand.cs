using Ardalis.GuardClauses;
using ECommerce.Services.Catalogs.Brands;
using ECommerce.Services.Catalogs.Shared.Contracts;
using ECommerce.Services.Catalogs.Shared.Extensions;
using MicroBootstrap.Abstractions.Core.Domain.Events.Internal;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductBrand.Events.Domain;

internal record ChangingProductBrand(BrandId BrandId) : DomainEvent;

internal class ChangingProductBrandValidationHandler :
    IDomainEventHandler<ChangingProductBrand>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ChangingProductBrandValidationHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(ChangingProductBrand notification, CancellationToken cancellationToken)
    {
        // Handling some validations
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.BrandId, nameof(notification.BrandId));
        Guard.Against.ExistsBrand(
            await _catalogDbContext.BrandExistsAsync(notification.BrandId, cancellationToken: cancellationToken),
            notification.BrandId);
    }
}
