using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Persistence;
using ECommerce.Services.Customers.Customers.Models;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.Customers.Features.UpdatingUserState.Events.External;

public record UserStateUpdated(Guid UserId, UserState OldUserState, UserState NewUserState)
    : IntegrationEvent, ITxRequest;

internal class UserStateUpdatedConsumer : IIntegrationEventHandler<UserStateUpdated>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<UserStateUpdatedConsumer> _logger;

    public UserStateUpdatedConsumer(
        CustomersDbContext customersDbContext,
        ILogger<UserStateUpdatedConsumer> logger)
    {
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task Handle(UserStateUpdated notification, CancellationToken cancellationToken)
    {
        var customer = await _customersDbContext.Customers
            .SingleOrDefaultAsync(x => x.IdentityId == notification.UserId, cancellationToken: cancellationToken);

        if (customer is null)
            return;

        switch (notification.NewUserState)
        {
            case UserState.Active:
                customer.Unlock();
                break;
            case UserState.Locked:
                customer.Lock();
                break;
            default:
                _logger.LogWarning("Received an unknown user state: '{NewUserState}'", notification.NewUserState);
                return;
        }

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            $"{(customer.IsActive ? "Unlocked" : "Locked")} " + $"customer with ID: '{customer.Id}'.");
    }
}
