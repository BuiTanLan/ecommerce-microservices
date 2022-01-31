using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.ValueObjects;
using BuildingBlocks.IdsGenerator;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Customers.ValueObjects;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Customers.Features.RegisteringUser.Events.External;

public record UserRegistered(
    Guid IdentityId,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles) : IntegrationEvent, ITxRequest;

internal class UserRegisteredConsumer : IIntegrationEventHandler<UserRegistered>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public UserRegisteredConsumer(CustomersDbContext customersDbContext, ILogger<UserRegisteredConsumer> logger)
    {
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task Handle(UserRegistered notification, CancellationToken cancellationToken)
    {
        if (notification.Roles.Contains(CustomersConstants.Role.User) == false)
            return;

        if (_customersDbContext.Customers.Any(x => x.IdentityId == notification.IdentityId))
            return;

        var customer = Customer.Create(
            new CustomerId(SnowFlakIdGenerator.NewId()),
            new Email(notification.Email),
            new Name(notification.FirstName, notification.LastName),
            notification.IdentityId);

        await _customersDbContext.AddAsync(customer, cancellationToken);

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created a new customer based on user with ID: '{IdentityId}'", notification.IdentityId);
    }
}
