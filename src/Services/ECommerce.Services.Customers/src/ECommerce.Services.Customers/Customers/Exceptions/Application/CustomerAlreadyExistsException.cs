using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions.Application;

internal class CustomerAlreadyExistsException : AppException
{
    public string? PhoneNumber { get; }
    public long? CustomerId { get; }
    public Guid? IdentityId { get; }

    public CustomerAlreadyExistsException(string phoneNumber)
        : base($"Customer with phoneNumber: '{phoneNumber}' already exists.")
    {
        PhoneNumber = phoneNumber;
    }

    public CustomerAlreadyExistsException(Guid identityId)
        : base($"Customer with IdentityId: '{identityId}' already exists.")
    {
        IdentityId = identityId;
    }

    public CustomerAlreadyExistsException(long customerId)
        : base($"Customer with ID: '{customerId}' already exists.")
    {
        CustomerId = customerId;
    }
}
