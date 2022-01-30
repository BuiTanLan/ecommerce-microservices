using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions;

internal class CannotVerifyCustomerException : AppException
{
    public long CustomerId { get; }

    public CannotVerifyCustomerException(long customerId)
        : base($"Customer with Id: '{customerId}' cannot be verified.")
    {
        CustomerId = customerId;
    }
}
