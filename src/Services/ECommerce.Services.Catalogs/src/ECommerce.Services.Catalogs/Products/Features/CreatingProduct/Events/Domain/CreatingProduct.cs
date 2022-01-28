using BuildingBlocks.Core.Domain.Events.Internal;
using ECommerce.Services.Catalogs.Brands;
using ECommerce.Services.Catalogs.Categories;
using ECommerce.Services.Catalogs.Products.Models;
using ECommerce.Services.Catalogs.Suppliers;

namespace ECommerce.Services.Catalogs.Products.Features.CreatingProduct.Events.Domain;

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
