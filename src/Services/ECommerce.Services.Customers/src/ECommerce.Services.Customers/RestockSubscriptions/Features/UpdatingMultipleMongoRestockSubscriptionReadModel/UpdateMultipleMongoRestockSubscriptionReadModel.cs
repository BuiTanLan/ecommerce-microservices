using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.CQRS.Command;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Read;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Write;
using ECommerce.Services.Customers.Shared.Data;
using MongoDB.Driver;

namespace ECommerce.Services.Customers.RestockSubscriptions.Features.UpdatingMultipleMongoRestockSubscriptionReadModel;

public record UpdateMultipleMongoRestockSubscriptionReadModel
    (IList<RestockSubscription> RestockSubscriptions) : InternalCommand;

internal class UpdateMultipleMongoRestockSubscriptionReadModelHandler
    : ICommandHandler<UpdateMultipleMongoRestockSubscriptionReadModel>
{
    private readonly IMapper _mapper;
    private readonly CustomersReadDbContext _customersReadDbContext;

    public UpdateMultipleMongoRestockSubscriptionReadModelHandler(
        IMapper mapper,
        CustomersReadDbContext customersReadDbContext)
    {
        _mapper = mapper;
        _customersReadDbContext = customersReadDbContext;
    }

    public async Task<Unit> Handle(
        UpdateMultipleMongoRestockSubscriptionReadModel command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var updatedEntities = _mapper.Map<IEnumerable<RestockSubscriptionReadModel>>(command.RestockSubscriptions);

        var listWrites = new List<WriteModel<RestockSubscriptionReadModel>>();

        // https://mongodb.github.io/mongo-csharp-driver/2.7/reference/driver/crud/writing/
        // https://mongodb.github.io/mongo-csharp-driver/2.7/reference/driver/crud/reading/
        // https://mongodb.github.io/mongo-csharp-driver/2.7/reference/driver/crud/linq/
        // https://mongodb.github.io/mongo-csharp-driver/2.7/reference/driver/crud/sessions_and_transactions/
        // https://fgambarino.com/c-sharp-mongo-bulk-write/
        foreach (var restockSubscriptionReadModel in updatedEntities)
        {
            var filterForUpdate =
                Builders<RestockSubscriptionReadModel>.Filter.Eq(x => x.Id, restockSubscriptionReadModel.Id);
            var updateDefinition =
                Builders<RestockSubscriptionReadModel>.Update.Set(x => x, restockSubscriptionReadModel);
            listWrites.Add(new UpdateOneModel<RestockSubscriptionReadModel>(filterForUpdate, updateDefinition));
        }

        await _customersReadDbContext.RestockSubscriptions.BulkWriteAsync(
            listWrites,
            cancellationToken: cancellationToken);

        listWrites.Clear();

        return Unit.Value;
    }
}
