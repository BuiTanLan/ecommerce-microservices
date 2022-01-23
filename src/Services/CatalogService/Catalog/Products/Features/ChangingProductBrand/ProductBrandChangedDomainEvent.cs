using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductBrand;

internal record ProductBrandChangedDomainEvent(long BrandId, long ProductId) : DomainEvent;
