using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidBirthDateException : DomainException
{
    public DateTime BirthDate { get; }

    public InvalidBirthDateException(string message) : base(message)
    {
    }

    public InvalidBirthDateException(DateTime birthDate) : base($"BirthDate: '{birthDate}' is invalid.")
    {
        BirthDate = birthDate;
    }
}
