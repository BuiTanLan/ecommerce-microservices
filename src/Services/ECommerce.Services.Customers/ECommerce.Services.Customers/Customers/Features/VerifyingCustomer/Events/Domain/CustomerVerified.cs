using Ardalis.GuardClauses;
using AutoMapper;
using ECommerce.Services.Customers.Customers.Features.UpdatingMongoCustomerReadsModel;
using ECommerce.Services.Customers.Customers.Models;
using MicroBootstrap.Abstractions.Core.Domain.Events.Internal;
using MicroBootstrap.Abstractions.CQRS.Command;
using MicroBootstrap.Core.Domain.Events.Internal;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer.Events.Domain;

public record CustomerVerified(Customer Customer) : DomainEvent;

internal class CustomerVerifiedHandler : IDomainEventHandler<CustomerVerified>
{
    private readonly IMapper _mapper;
    private readonly ICommandProcessor _commandProcessor;

    public CustomerVerifiedHandler(IMapper mapper, ICommandProcessor commandProcessor)
    {
        _mapper = mapper;
        _commandProcessor = commandProcessor;
    }

    public Task Handle(CustomerVerified notification, CancellationToken cancellationToken)
    {
        Guard.Against.Null(notification, nameof(notification));

        var command = _mapper.Map<UpdateMongoCustomerReadsModel>(notification.Customer);

        return _commandProcessor.ScheduleAsync(command, cancellationToken);
    }
}
