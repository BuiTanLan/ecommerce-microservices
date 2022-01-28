using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidFullNameException : DomainException
{
    public string FullName { get; }

    public InvalidFullNameException(string fullName) : base($"Full name: '{fullName}' is invalid.")
    {
        FullName = fullName;
    }
}
