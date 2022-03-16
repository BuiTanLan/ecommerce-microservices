using ECommerce.Services.Catalogs.Brands;
using ECommerce.Services.Catalogs.Products.ValueObjects;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Catalogs.Products.Features.ChangingProductBrand.Events.Domain;

internal record ProductBrandChanged(BrandId BrandId, ProductId ProductId) : DomainEvent;
