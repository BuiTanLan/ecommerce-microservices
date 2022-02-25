using BuildingBlocks.Persistence.Mongo;

namespace BuildingBlocks.Abstractions.Persistence.Mongo;

public interface IMongoUnitOfWork<TContext> : IUnitOfWork<TContext>
    where TContext : MongoDbContext
{
    void AddCommand(Func<Task> func);
}
