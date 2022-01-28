using BuildingBlocks.Core.Domain.Exceptions;

namespace ECommerce.Services.Customers.Customers.Exceptions;

public class InvalidAmountException : DomainException
{
    public decimal Amount { get; }

    public InvalidAmountException(decimal amount) : base($"Amount: '{amount}' is invalid.")
    {
        Amount = amount;
    }
}
