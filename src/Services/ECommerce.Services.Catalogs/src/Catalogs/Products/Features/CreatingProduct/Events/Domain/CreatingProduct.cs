using BuildingBlocks.Core.Domain.Events.Internal;
using Catalogs.Brands;
using Catalogs.Categories;
using Catalogs.Products.Models;
using Catalogs.Suppliers;

namespace Catalogs.Products.Features.CreatingProduct.Events.Domain;

public record CreatingProduct(
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
