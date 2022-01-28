using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class UnsupportedNationalityException : DomainException
{
    public string Nationality { get; }

    public UnsupportedNationalityException(string nationality) : base($"Nationality: '{nationality}' is unsupported.")
    {
        Nationality = nationality;
    }
}
