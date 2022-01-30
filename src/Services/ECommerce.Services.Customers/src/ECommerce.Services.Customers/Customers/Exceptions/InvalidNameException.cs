using BuildingBlocks.Exception.Types;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidNameException : BadRequestException
{
    public string Name { get; }

    public InvalidNameException(string name) : base($"Name: '{name}' is invalid.")
    {
        Name = name;
    }
}
