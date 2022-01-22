namespace Catalog.Products.Models;

public class ProductView
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public long? SupplierId { get; set; }
    public string? SupplierName { get; set; }
}
