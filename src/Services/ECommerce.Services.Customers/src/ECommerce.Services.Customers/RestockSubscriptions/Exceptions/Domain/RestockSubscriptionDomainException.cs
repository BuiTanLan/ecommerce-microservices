using BuildingBlocks.Abstractions.Domain.Exceptions;

namespace ECommerce.Services.Customers.RestockSubscriptions.Exceptions.Domain;

public class RestockSubscriptionDomainException : DomainException
{
    public RestockSubscriptionDomainException(string message) : base(message)
    {
    }
}
