using ECommerce.Services.Orders.Orders.ValueObjects;

namespace ECommerce.Services.Orders.Orders;

public class Order
{
    public CustomerInfo Customer { get; private set; }
    public ProductInfo Product { get; private set; }
}
