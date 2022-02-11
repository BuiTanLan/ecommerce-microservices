using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.DeletingRestockSubscriptionsByTime;

public record DeleteRestockSubscriptionsByTime(DateTime? From = null, DateTime? To = null) : ITxCommand;

internal class DeleteRestockSubscriptionsByTimeHandler : ICommandHandler<DeleteRestockSubscriptionsByTime>
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly ILogger<DeleteRestockSubscriptionsByTimeHandler> _logger;

    public DeleteRestockSubscriptionsByTimeHandler(
        CustomersDbContext customersDbContext,
        ILogger<DeleteRestockSubscriptionsByTimeHandler> logger)
    {
        _customersDbContext = customersDbContext;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteRestockSubscriptionsByTime command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var exists = await _customersDbContext.RestockSubscriptions
            .Where(x => (command.From == null && command.To == null) ||
                        (command.From == null && x.Created <= command.To) ||
                        (command.To == null && x.Created >= command.From) ||
                        (x.Created >= command.From && x.Created <= command.To))
            .ToListAsync(cancellationToken: cancellationToken);

        if (exists.Any() == false)
            return Unit.Value;

        _customersDbContext.RestockSubscriptions.RemoveRange(exists);
        await _customersDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("'{Count}' RestockSubscriptions removed.'", exists.Count);

        return Unit.Value;
    }
}
