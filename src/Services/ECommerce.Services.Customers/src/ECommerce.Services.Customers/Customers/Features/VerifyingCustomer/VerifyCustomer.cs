using BuildingBlocks.Abstractions.CQRS.Command;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Customers.Exceptions;
using ECommerce.Services.Customers.Customers.Exceptions.Application;
using ECommerce.Services.Customers.Shared.Data;
using ECommerce.Services.Customers.Shared.Extensions;

namespace ECommerce.Services.Customers.Customers.Features.VerifyingCustomer;

public record VerifyCustomer(long CustomerId) : ITxCommand;

internal class VerifyCustomerValidator : AbstractValidator<VerifyCustomer>
{
    public VerifyCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}

internal class VerifyCustomerHandler : ICommandHandler<VerifyCustomer>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<VerifyCustomerHandler> _logger;

    public VerifyCustomerHandler(CustomersDbContext customersDbContext, ILogger<VerifyCustomerHandler> logger)
    {
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(VerifyCustomer command, CancellationToken cancellationToken)
    {
        var customer = await _customersDbContext.FindCustomerByIdAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }

        customer.Verify(DateTime.Now);

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Verified a customer with ID: '{CustomerId}'", command.CustomerId);

        return Unit.Value;
    }
}
