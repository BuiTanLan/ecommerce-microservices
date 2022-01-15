using System.Collections.Concurrent;
using System.Transactions;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IsolationLevel = System.Data.IsolationLevel;

namespace BuildingBlocks.EFCore;

// https://github.com/Daniel127/EF-Unit-Of-Work
public class EfUnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
    where TDbContext : AppDbContextBase
{
    private readonly TDbContext _context;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EfUnitOfWork<TDbContext>> _logger;
    private readonly IDictionary<Type, object> _repositories;

    public EfUnitOfWork(
        TDbContext context,
        IDomainEventDispatcher domainEventDispatcher,
        IServiceProvider serviceProvider,
        ILogger<EfUnitOfWork<TDbContext>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainEventDispatcher =
            domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;

        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
    {
        await _context.BeginTransactionAsync(isolationLevel);
    }

    public async Task CommitTransactionAsync()
    {
        // dispatch domain events before commiting transaction
        await _domainEventDispatcher.DispatchEventsAsync();

        await _context.CommitTransactionAsync();
    }

    public void RollbackTransaction()
    {
        _context.RollbackTransaction();
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

    public async Task RetryOnExceptionAsync(Func<Task> operation)
    {
        await _context.Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public TDbContext DbContext { get; }
}
