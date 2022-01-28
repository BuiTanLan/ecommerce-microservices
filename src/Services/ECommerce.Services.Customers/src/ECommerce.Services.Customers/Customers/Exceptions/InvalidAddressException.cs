using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidAddressException : DomainException
{
    public string Address { get; }

    public InvalidAddressException(string address) : base($"Address: '{address}' is invalid.")
    {
        Address = address;
    }
}
