using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Categories;
using Catalog.Core.Contracts;
using Catalog.Infrastructure.Extensions;

namespace Catalog.Products.Features.CreateProduct.Events.Domain.Handlers;

internal class ValidateCategoryExistsDomainEventHandler :
    IDomainEventHandler<CreatingProductDomainEvent>,
    IDomainEventHandler<ChangingProductCategoryDomainEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ValidateCategoryExistsDomainEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task Handle(CreatingProductDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.CategoryId, nameof(notification.CategoryId));
        Guard.Against.CategoryNotFound(
            await _catalogDbContext.CategoryExistsAsync(notification.Id, cancellationToken: cancellationToken),
            notification.Id);
    }

    public async Task Handle(ChangingProductCategoryDomainEvent notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));
        Guard.Against.NegativeOrZero(notification.CategoryId, nameof(notification.CategoryId));
        Guard.Against.CategoryNotFound(
            await _catalogDbContext.CategoryExistsAsync(notification.CategoryId, cancellationToken: cancellationToken),
            notification.CategoryId);
    }
}
