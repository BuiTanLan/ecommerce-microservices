using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Core.Domain.ValueObjects;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.ValueObjects;
using ECommerce.Services.Customers.RestockSubscriptions.Exceptions.Domain;
using ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription;
using ECommerce.Services.Customers.RestockSubscriptions.ValueObjects;

namespace ECommerce.Services.Customers.RestockSubscriptions.Models;

public class RestockSubscription : AggregateRoot<RestockSubscriptionId>
{
    public CustomerId CustomerId { get; private set; } = null!;
    public Customer? Customer { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public ProductInformation ProductInformation { get; private set; } = null!;

    public static RestockSubscription Create(
        RestockSubscriptionId id,
        CustomerId customerId,
        ProductInformation productInformation,
        Email email)
    {
        Guard.Against.Null(id, new RestockSubscriptionDomainException("Id cannot be null"));
        Guard.Against.Null(customerId, new RestockSubscriptionDomainException("CustomerId cannot be null"));
        Guard.Against.Null(email, new RestockSubscriptionDomainException("Email cannot be null"));
        Guard.Against.Null(
            productInformation,
            new RestockSubscriptionDomainException("ProductInformation cannot be null"));

        var restockSubscription = new RestockSubscription
        {
            Id = id,
            Email = email,
            CustomerId = customerId,
            ProductInformation = productInformation
        };

        restockSubscription.AddDomainEvent(new RestockSubscriptionCreated(restockSubscription));

        return restockSubscription;
    }
}
