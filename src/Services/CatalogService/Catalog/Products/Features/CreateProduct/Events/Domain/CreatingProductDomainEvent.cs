using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Products.Models;

namespace Catalog.Products.Features.CreateProduct.Events.Domain;

public record CreatingProductDomainEvent(
    long Id,
    string Name,
    decimal Price,
    int Stock,
    int RestockThreshold,
    int MaxStockThreshold,
    ProductStatus Status,
    int Width,
    int Height,
    int Depth,
    long CategoryId,
    long SupplierId,
    long BrandId,
    string? Description = null) : DomainEvent;
