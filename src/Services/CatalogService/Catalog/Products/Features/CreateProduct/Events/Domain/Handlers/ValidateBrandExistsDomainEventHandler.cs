using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Brands;
using Catalog.Core.Contracts;
using Catalog.Infrastructure.Extensions;

namespace Catalog.Products.Features.CreateProduct.Events.Domain.Handlers;

internal class ValidateBrandExistsDomainEventHandler :
    IDomainEventHandler<CreatingProductDomainEvent>,
    IDomainEventHandler<ChangingProductBrandDomainEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ValidateBrandExistsDomainEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(CreatingProductDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.BrandId, nameof(notification.BrandId));
        Guard.Against.BrandNotFound(
            await _catalogDbContext.BrandExistsAsync(notification.Id, cancellationToken: cancellationToken),
            notification.Id);
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
