namespace Catalog.Products.Features.GetProductsView;

public record struct ProductViewDto(long Id, string Name, string CategoryName, string SupplierName, long ItemCount);
