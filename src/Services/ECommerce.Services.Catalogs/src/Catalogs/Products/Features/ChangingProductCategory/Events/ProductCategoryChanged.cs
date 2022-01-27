using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Features.ChangingProductCategory.Events;

public record ProductCategoryChanged(long NewCategoryId, long OldCategoryId) : DomainEvent;
