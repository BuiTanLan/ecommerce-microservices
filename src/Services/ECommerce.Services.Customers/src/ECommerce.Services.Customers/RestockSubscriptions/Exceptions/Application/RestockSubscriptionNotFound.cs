using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.RestockSubscriptions.Exceptions.Application;

public class RestockSubscriptionNotFound : NotFoundException
{
    public RestockSubscriptionNotFound(long id) : base("RestockSubscription with id: " + id + " not found")
    {
    }
}
