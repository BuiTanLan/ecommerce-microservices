using BuildingBlocks.CQRS.Command;
using BuildingBlocks.IdsGenerator;
using Catalog.Products.Features.CreateProduct.Requests;
using Catalog.Products.Models;

namespace Catalog.Products.Features.CreateProduct.Command;

public record CreateProductCommand(
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
    string? Description = null,
    IEnumerable<CreateProductImageRequest>? Images = null) : ICommand<CreateProductCommandResult>
{
    public long Id { get; } = SnowFlakIdGenerator.NewId();
}
