using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.RestockSubscriptions.ValueObjects.Exceptions;

public class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(long id) : base($"Product with id {id} not found")
    {
    }
}
