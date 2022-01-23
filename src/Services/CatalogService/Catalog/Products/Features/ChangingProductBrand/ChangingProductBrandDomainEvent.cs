using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Brands;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;

namespace Catalog.Products.Features.ChangingProductBrand;

internal record ChangingProductBrandDomainEvent(long BrandId) : DomainEvent;

internal class ValidateBrandExistsDomainEventHandler :
    IDomainEventHandler<ChangingProductBrandDomainEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ValidateBrandExistsDomainEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(ChangingProductBrandDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.BrandId, nameof(notification.BrandId));
        Guard.Against.BrandNotFound(
            await _catalogDbContext.BrandExistsAsync(notification.BrandId, cancellationToken: cancellationToken),
            notification.BrandId);
    }
}
