using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions.Domain;

internal class CustomerAlreadyVerifiedException : AppException
{
    public long CustomerId { get; }

    public CustomerAlreadyVerifiedException(long customerId)
        : base($"Customer with Id: '{customerId}' cannot be verified.")
    {
        CustomerId = customerId;
    }
}
