using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomer;

namespace ECommerce.Services.Customers.Identity.Features.RegisteringUser.Events.External;

public record UserRegistered(string Email, List<string> Roles) : IntegrationEvent, ITxRequest;

internal class UserRegisteredConsumer : IIntegrationEventHandler<UserRegistered>
{
    private readonly ICommandProcessor _commandProcessor;

    public UserRegisteredConsumer(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task Handle(UserRegistered notification, CancellationToken cancellationToken)
    {
        if (notification.Roles.Contains(CustomersConstants.Role.User) == false)
            return;

        await _commandProcessor.SendAsync(new CreateCustomer(notification.Email), cancellationToken);
    }
}
