using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidPhoneNumberException : DomainException
{
    public string PhoneNumber { get; }

    public InvalidPhoneNumberException(string phoneNumber) : base($"PhoneNumber: '{phoneNumber}' is invalid.")
    {
        PhoneNumber = phoneNumber;
    }
}
