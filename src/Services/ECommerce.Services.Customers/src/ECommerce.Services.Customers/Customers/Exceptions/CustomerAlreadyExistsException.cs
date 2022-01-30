using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions;

internal class CustomerAlreadyExistsException : AppException
{
    public string? PhoneNumber { get; }
    public long? CustomerId { get; }

    public CustomerAlreadyExistsException(string phoneNumber)
        : base($"Customer with phoneNumber: '{phoneNumber}' already exists.")
    {
        PhoneNumber = phoneNumber;
    }

    public CustomerAlreadyExistsException(long customerId)
        : base($"Customer with ID: '{customerId}' already exists.")
    {
        CustomerId = customerId;
    }
}
