using System.Collections.Immutable;
using System.Data;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EFCore;

public abstract class AppDbContextBase :
    DbContext,
    IDomainEventContext,
    IDbFacadeResolver,
    IDbContext,
    ITxDbContextExecutes
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private IDbContextTransaction _currentTransaction;

    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    protected AppDbContextBase(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher) : base(options)
    {
        _domainEventDispatcher = Guard.Against.Null(domainEventDispatcher, nameof(domainEventDispatcher));
        System.Diagnostics.Debug.WriteLine($"{GetType().Name}::ctor");
    }

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // https://github.com/dotnet-architecture/eShopOnContainers/blob/e05a87658128106fef4e628ccb830bc89325d9da/src/Services/Ordering/Ordering.Infrastructure/OrderingContext.cs#L65
        // https://github.com/dotnet-architecture/eShopOnContainers/issues/700#issuecomment-461807560
        // http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/
        // http://www.kamilgrzybek.com/design/handling-domain-events-missing-part/
        // https://youtu.be/x-UXUGVLMj8?t=4515
        // https://enterprisecraftsmanship.com/posts/domain-events-simple-reliable-solution/
        // https://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/
        // https://www.ledjonbehluli.com/posts/domain_to_integration_event/
        // https://ardalis.com/immediate-domain-event-salvation-with-mediatr/
        await _domainEventDispatcher.DispatchAsync(cancellationToken);

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
        // performed through the DbContext will be committed
        var result = await SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task RetryOnExceptionAsync(Func<Task> operation)
    {
        return Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
    {
        return Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IHaveAggregate>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToImmutableList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return domainEvents.ToImmutableList();
    }

    public IReadOnlyList<(IHaveAggregate Aggregate, IReadOnlyList<IDomainEvent> DomainEvents)>
        GetAggregateDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IHaveAggregate>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var aggregates = domainEntities
            .GroupBy(x => x)
            .Select(x => (Aggregate: x.Key,
                DomainEvents: (IReadOnlyList<IDomainEvent>)x.Key.DomainEvents.ToList().AsReadOnly()))
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return aggregates;
    }

    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
