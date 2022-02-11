using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Read;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Write;
using ECommerce.Services.Customers.Shared.Data;
using MongoDB.Driver;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.UpdatingMongoRestockSubscriptionReadModel;

public record UpdateMongoRestockSubscriptionReadModel
    (RestockSubscription RestockSubscription, bool IsDeleted) : InternalCommand;

internal class UpdateMongoRestockSubscriptionReadModelHandler : ICommandHandler<UpdateMongoRestockSubscriptionReadModel>
{
    private readonly CustomersReadDbContext _customersReadDbContext;
    private readonly IMapper _mapper;

    public UpdateMongoRestockSubscriptionReadModelHandler(CustomersReadDbContext customersReadDbContext, IMapper mapper)
    {
        _customersReadDbContext = customersReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateMongoRestockSubscriptionReadModel command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var updatedEntity = new RestockSubscriptionReadModel()
        {
            Created = command.RestockSubscription.Created,
            Email = command.RestockSubscription.Email.Value,
            Processed = command.RestockSubscription.Processed,
            CustomerId = command.RestockSubscription.CustomerId.Value,
            IsDeleted = command.IsDeleted,
            ProcessedTime = command.RestockSubscription.ProcessedTime,
            ProductId = command.RestockSubscription.ProductInformation.Id.Value,
            ProductName = command.RestockSubscription.ProductInformation.Name,
            RestockSubscriptionId = command.RestockSubscription.Id.Value
        };

        await _customersReadDbContext.RestockSubscriptions.ReplaceOneAsync(
            x => x.RestockSubscriptionId == command.RestockSubscription.Id.Value,
            updatedEntity,
            new ReplaceOptions(),
            cancellationToken);

        return Unit.Value;
    }
}
