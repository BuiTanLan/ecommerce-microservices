using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription.Exceptions;

public class ProductAlreadyHasAvailableStockException : AppException
{
    public ProductAlreadyHasAvailableStockException(long productId, int quantity, string name) : base(
        $@"Product with Id '{productId}' and name '{name}' already has available stock of '{quantity}' items.")
    {
    }
}
