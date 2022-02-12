using BuildingBlocks.Core.Persistence;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace BuildingBlocks.Mongo;

// https://www.thecodebuzz.com/mongodb-repository-implementation-unit-testing-net-core-example/
public class MongoDbContext : IMongoDbContext, ITxDbContextExecutes
{
    public IClientSessionHandle? Session { get; set; }
    public IMongoDatabase Database { get; }
    public IMongoClient MongoClient { get; }

    public MongoDbContext(MongoOptions options)
    {
        // Set Guid to CSharp style (with dash -)
        BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;

        RegisterConventions();

        MongoClient = new MongoClient(options.ConnectionString);
        var databaseName = options.DatabaseName;
        Database = MongoClient.GetDatabase(databaseName);
    }

    private static void RegisterConventions()
    {
        ConventionRegistry.Register(
            "conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreIfDefaultConvention(false),
                new ImmutablePocoConvention()
            }, _ => true);
    }

    public IMongoCollection<T> GetCollection<T>(string? name = null)
    {
        return Database.GetCollection<T>(name ?? typeof(T).Name.ToLower());
    }

    public void Dispose()
    {
        while (Session is { IsInTransaction: true })
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

        GC.SuppressFinalize(this);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        Session.StartTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Session is { IsInTransaction: true })
            await Session.CommitTransactionAsync(cancellationToken);

        Session?.Dispose();
    }

    public async Task RollbackTransaction(CancellationToken cancellationToken = default)
    {
        await Session?.AbortTransactionAsync(cancellationToken)!;
    }

    public async Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await action();

            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
    }

    public async Task<T> ExecuteTransactionalAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();

            await CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
    }
}
