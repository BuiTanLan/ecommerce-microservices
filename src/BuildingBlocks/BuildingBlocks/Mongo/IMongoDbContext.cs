using MongoDB.Driver;

namespace BuildingBlocks.Mongo
{
    public interface IMongoDbContext : IDisposable
    {
        IMongoDatabase Database { get; }
        IMongoClient MongoClient { get; }
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransaction(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        IMongoCollection<T> GetCollection<T>(string? name = null);
    }
}
