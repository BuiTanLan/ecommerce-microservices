using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions.Application;

internal class CannotCompleteCustomerException : AppException
{
    public long CustomerId { get; }

    public CannotCompleteCustomerException(long customerId)
        : base($"Customer with ID: '{customerId}' cannot be completed.")
    {
        CustomerId = customerId;
    }
}
