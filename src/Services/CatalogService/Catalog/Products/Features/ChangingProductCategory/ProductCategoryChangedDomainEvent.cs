using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductCategory;

public record ProductCategoryChangedDomainEvent(long CategoryId, long ProductId) : DomainEvent;
