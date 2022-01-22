namespace Catalog.Products.Features.CreateProduct.Requests;

public record CreateProductImageRequest
{
    public string ImageUrl { get; init; } = default!;
    public bool IsMain { get; init; }
}
