using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidNationalityException : DomainException
{
    public string Nationality { get; }

    public InvalidNationalityException(string nationality) : base($"Nationality: '{nationality}' is invalid.")
    {
        Nationality = nationality;
    }
}
