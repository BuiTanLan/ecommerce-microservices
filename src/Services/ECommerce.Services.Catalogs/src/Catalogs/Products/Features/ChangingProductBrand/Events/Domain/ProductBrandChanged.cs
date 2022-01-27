using BuildingBlocks.Core.Domain.Events.Internal;

namespace Catalogs.Products.Features.ChangingProductBrand.Events.Domain;

internal record ProductBrandChanged(long BrandId, long ProductId) : DomainEvent;
