using BuildingBlocks.Abstractions.Domain.Model;
using BuildingBlocks.Abstractions.Persistence.Mongo;

namespace BuildingBlocks.Persistence.Mongo.Repositories;

public class MongoEventSourcedRepository<TEntity> : IMongoEventSourcedRepository<TEntity>
    where TEntity : class, IAggregate<Guid>, IHaveIdentity<Guid>, new()
{
    private readonly IMongoDbContext _mongoDbContext;

    public MongoEventSourcedRepository(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public Task<TEntity?> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> Update(TEntity entity, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(TEntity entity, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteById(Guid id, int? expectedVersion, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _mongoDbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
