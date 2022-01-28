using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidNameException : DomainException
{
    public string Name { get; }

    public InvalidNameException(string name) : base($"Name: '{name}' is invalid.")
    {
        Name = name;
    }
}
