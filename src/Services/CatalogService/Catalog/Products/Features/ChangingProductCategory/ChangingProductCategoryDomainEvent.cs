using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Categories;
using Catalog.Shared.Core.Contracts;
using Catalog.Shared.Infrastructure.Extensions;

namespace Catalog.Products.Features.ChangingProductCategory;

public record ChangingProductCategoryDomainEvent(long CategoryId) : DomainEvent;

internal class ValidateCategoryExistsDomainEventHandler :
    IDomainEventHandler<ChangingProductCategoryDomainEvent>
{
    private readonly ICatalogDbContext _catalogDbContext;

    public ValidateCategoryExistsDomainEventHandler(ICatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
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
