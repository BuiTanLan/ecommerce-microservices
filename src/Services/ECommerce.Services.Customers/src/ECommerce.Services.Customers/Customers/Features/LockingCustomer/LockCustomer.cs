using BuildingBlocks.Core.Domain;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Exceptions;
using ECommerce.Services.Customers.Customers.Exceptions.Application;
using ECommerce.Services.Customers.Shared.Data;
using ECommerce.Services.Customers.Shared.Extensions;

namespace ECommerce.Services.Customers.Customers.Features.LockingCustomer;

public record LockCustomer(long CustomerId, string? Notes = null) : ICommand, ITxRequest;

internal class LockCustomerValidator : AbstractValidator<LockCustomer>
{
    public LockCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}

internal class LockCustomerHandler : ICommandHandler<LockCustomer>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<LockCustomerHandler> _logger;

    public LockCustomerHandler(CustomersDbContext customersDbContext, ILogger<LockCustomerHandler> logger)
    {
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(LockCustomer command, CancellationToken cancellationToken)
    {
        var customer = await _customersDbContext.FindCustomerByIdAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }

        customer.Lock(command.Notes);
        await _customersDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Locked a customer with ID: '{CustomerId}'", command.CustomerId);

        return Unit.Value;
    }
}
