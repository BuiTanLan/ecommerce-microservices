using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using BuildingBlocks.Exception;
using ECommerce.Services.Customers.RestockSubscriptions.Exceptions.Application;
using ECommerce.Services.Customers.RestockSubscriptions.ValueObjects;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.MarkingRestockSubscriptionAsProcessed;

public record MarkRestockSubscriptionAsProcessed(RestockSubscriptionId Id) : ITxCommand;

internal class MarkRestockSubscriptionAsProcessedValidator : AbstractValidator<MarkRestockSubscriptionAsProcessed>
{
    public MarkRestockSubscriptionAsProcessedValidator()
    {
        RuleFor(command => command.Id)
            .NotNull();
    }
}

internal class MarkRestockSubscriptionAsProcessedHandler : ICommandHandler<MarkRestockSubscriptionAsProcessed>
{
    private readonly CustomersDbContext _customersDbContext;

    public MarkRestockSubscriptionAsProcessedHandler(CustomersDbContext customersDbContext)
    {
        _customersDbContext = customersDbContext;
    }

    public async Task<Unit> Handle(MarkRestockSubscriptionAsProcessed command, CancellationToken cancellationToken)
    {
        var restockSubscription = await _customersDbContext.RestockSubscriptions.FindAsync(command.Id);
        Guard.Against.Null(restockSubscription, new RestockSubscriptionNotFoundException(command.Id));

        restockSubscription!.MarkAsProcessed(DateTime.Now);

        await _customersDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
