using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class CustomerNotFoundException : NotFoundException
{
    public CustomerNotFoundException(string message) : base(message)
    {
    }

    public CustomerNotFoundException(long id) : base($"Customer with id '{id}' not found.")
    {
    }
}
