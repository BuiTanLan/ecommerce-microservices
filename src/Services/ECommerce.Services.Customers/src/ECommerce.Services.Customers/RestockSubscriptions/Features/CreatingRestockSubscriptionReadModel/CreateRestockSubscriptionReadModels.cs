using Ardalis.GuardClauses;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Read;
using ECommerce.Services.Customers.Shared.Data;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscriptionReadModel;

public record CreateRestockSubscriptionReadModels(
    long CustomerId,
    string CustomerName,
    long ProductId,
    string ProductName,
    bool Processed,
    DateTime? ProcessedTime = null) : ICreateCommand;

internal class CreateRestockSubscriptionReadModelHandler : ICommandHandler<CreateRestockSubscriptionReadModels>
{
    private readonly CustomersReadDbContext _mongoDbContext;

    public CreateRestockSubscriptionReadModelHandler(CustomersReadDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<Unit> Handle(CreateRestockSubscriptionReadModels command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        await CreatingMongoDbReadModel(command, cancellationToken);

        return Unit.Value;
    }

    private Task CreatingMongoDbReadModel(
        CreateRestockSubscriptionReadModels command,
        CancellationToken cancellationToken)
    {
        var readModel = new RestockSubscriptionReadModel(
            command.CustomerId,
            command.CustomerName,
            command.ProductId,
            command.ProductName,
            command.Processed,
            command.ProcessedTime);

        return _mongoDbContext.RestockSubscriptions.InsertOneAsync(readModel, cancellationToken: cancellationToken);
    }
}
