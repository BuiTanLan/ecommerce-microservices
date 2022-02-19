using System.Collections.Concurrent;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IsolationLevel = System.Data.IsolationLevel;

namespace BuildingBlocks.EFCore;

// https://github.com/Daniel127/EF-Unit-Of-Work
public class EfUnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
    where TDbContext : AppDbContextBase
{
    private readonly TDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EfUnitOfWork<TDbContext>> _logger;
    private readonly IDictionary<Type, object> _repositories;

    public EfUnitOfWork(
        TDbContext context,
        IServiceProvider serviceProvider,
        ILogger<EfUnitOfWork<TDbContext>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;

        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public TDbContext DbContext => _context;

    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class
    {
        return _context.Set<TEntity>();
    }

    public Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
    {
        return _context.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.RollbackTransactionAsync(cancellationToken);
    }

    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
        // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
        return _context.SaveEntitiesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : class, IAggregateRoot<TKey>
    {
        Type entityType = typeof(TEntity);
        if (_repositories.ContainsKey(entityType))
        {
            return (IRepository<TEntity, TKey>)_repositories[entityType];
        }

        try
        {
            IRepository<TEntity, TKey> customRepo =
                (IRepository<TEntity, TKey>)_serviceProvider.GetService(typeof(IRepository<TEntity, TKey>));
            _repositories[entityType] = customRepo ?? throw new System.Exception("Service null");
            return customRepo;
        }
        catch (System.Exception e)
        {
            _logger?.LogDebug("Can't get Repository from service provider: {0}", e.Message);
        }

        _repositories[entityType] = new Repository<TDbContext, TEntity, TKey>(_context);
        return (IRepository<TEntity, TKey>)_repositories[entityType];
    }

    public IRepository<TEntity, long> GetRepository<TEntity>()
        where TEntity : class, IAggregateRoot<long>
    {
        return GetRepository<TEntity, long>();
    }

    public Task RetryOnExceptionAsync(Func<Task> operation)
    {
        return _context.RetryOnExceptionAsync(operation);
    }

    public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
    {
        return _context.RetryOnExceptionAsync(operation);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        return _context.ExecuteTransactionalAsync(action, cancellationToken);
    }

    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        return _context.ExecuteTransactionalAsync(action, cancellationToken);
    }
}
