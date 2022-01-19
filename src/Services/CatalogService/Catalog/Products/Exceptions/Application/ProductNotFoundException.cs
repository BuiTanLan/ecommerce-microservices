using BuildingBlocks.Exception;

namespace Catalog.Products.Exceptions.Application;

public class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(string id) : base($"Product with id '{id}' not found")
    {
    }
}
