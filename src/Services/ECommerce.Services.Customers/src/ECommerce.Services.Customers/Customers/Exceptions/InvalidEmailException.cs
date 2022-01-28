using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidEmailException : DomainException
{
    public string Email { get; }

    public InvalidEmailException(string email) : base($"Email: '{email}' is invalid.")
    {
        Email = email;
    }
}
