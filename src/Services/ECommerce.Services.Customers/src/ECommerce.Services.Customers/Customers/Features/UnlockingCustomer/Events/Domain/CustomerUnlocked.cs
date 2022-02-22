using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.Abstractions.Domain.Events.Internal;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Features.UpdatingMongoCustomerReadsModel;
using ECommerce.Services.Customers.Customers.Models;

namespace ECommerce.Services.Customers.Customers.Features.UnlockingCustomer.Events.Domain;

public record CustomerUnlocked(Customer Customer) : DomainEvent;

internal class CustomerUnlockedHandler : IDomainEventHandler<CustomerUnlocked>
{
    private readonly IMapper _mapper;
    private readonly ICommandProcessor _commandProcessor;

    public CustomerUnlockedHandler(IMapper mapper, ICommandProcessor commandProcessor)
    {
        _mapper = mapper;
        _commandProcessor = commandProcessor;
    }

    public Task Handle(CustomerUnlocked notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var command = _mapper.Map<UpdateMongoCustomerReadsModel>(notification.Customer);

        return _commandProcessor.ScheduleAsync(command, cancellationToken);
    }
}
