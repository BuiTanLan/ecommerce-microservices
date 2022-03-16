using ECommerce.Services.Customers.Customers.Features.CreatingCustomer;
using MicroBootstrap.Abstractions.Core.Domain.Events.External;
using MicroBootstrap.Abstractions.CQRS.Command;
using MicroBootstrap.Abstractions.Persistence;
using MicroBootstrap.Core.Domain.Events.External;

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
