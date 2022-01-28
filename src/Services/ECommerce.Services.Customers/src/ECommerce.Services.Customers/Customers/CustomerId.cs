using BuildingBlocks.Core.Domain.Model;

namespace ECommerce.Services.Customers.Customers;

public class CustomerId : AggregateId
{
    public CustomerId(long value) : base(value)
    {
    }

    public static implicit operator long(CustomerId id) => id.Value;

    public static implicit operator CustomerId(long id) => new(id);
}
