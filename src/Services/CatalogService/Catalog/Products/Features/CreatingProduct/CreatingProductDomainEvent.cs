using BuildingBlocks.Core.Domain.Events.Internal;
using Catalog.Brands;
using Catalog.Categories;
using Catalog.Products.Models;
using Catalog.Suppliers;

namespace Catalog.Products.Features.CreatingProduct;

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
    Category? Category,
    Supplier? Supplier,
    Brand? Brand,
    string? Description = null) : DomainEvent;
