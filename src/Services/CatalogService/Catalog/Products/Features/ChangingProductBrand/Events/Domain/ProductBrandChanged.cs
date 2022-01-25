using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalog.Products.Features.ChangingProductBrand.Events.Domain;

internal record ProductBrandChanged(long BrandId, long ProductId) : DomainEvent;
