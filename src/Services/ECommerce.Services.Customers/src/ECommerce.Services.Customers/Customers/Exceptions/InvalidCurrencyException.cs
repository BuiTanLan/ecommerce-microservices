using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidCurrencyException : DomainException
{
    public string Currency { get; }

    public InvalidCurrencyException(string currency) : base($"Currency: '{currency}' is invalid.")
    {
        Currency = currency;
    }
}
