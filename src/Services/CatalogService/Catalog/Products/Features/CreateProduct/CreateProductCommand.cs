using BuildingBlocks.CQRS.Command;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    long CategoryId,
    long SupplierId) : ICommand<CreateProductCommandResult>;
