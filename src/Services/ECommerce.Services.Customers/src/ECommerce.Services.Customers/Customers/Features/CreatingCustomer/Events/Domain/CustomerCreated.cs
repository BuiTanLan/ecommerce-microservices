using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Features.CreatingCustomersReadModels;
using ECommerce.Services.Customers.Customers.Models;

namespace ECommerce.Services.Customers.Customers.Features.CreatingCustomer.Events.Domain;

public record CustomerCreated(Customer Customer) : DomainEvent;

internal class CustomerCreatedHandler : IDomainEventHandler<CustomerCreated>
{
    private readonly ICommandProcessor _commandProcessor;
    private readonly IMapper _mapper;

    public CustomerCreatedHandler(ICommandProcessor commandProcessor, IMapper mapper)
    {
        _commandProcessor = commandProcessor;
        _mapper = mapper;
    }

    public Task Handle(CustomerCreated notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var command = _mapper.Map<CreateCustomerReadModels>(notification.Customer);

        return _commandProcessor.SendAsync(command, cancellationToken);
    }
}
