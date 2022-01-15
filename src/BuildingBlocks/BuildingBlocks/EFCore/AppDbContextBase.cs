using System.Collections.Immutable;
using System.Data;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EFCore;

public abstract class AppDbContextBase : DbContext, IDomainEventContext, IDbFacadeResolver
{
    private IDbContextTransaction _currentTransaction;

    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(isolationLevel);
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            await _currentTransaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RetryOnExceptionAsync(Func<Task> operation)
    {
        await Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x =>
                x.Entity.DomainEvents != null &&
                x.Entity.DomainEvents.Any())
            .ToImmutableList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToImmutableList();

        domainEntities.ForEach(entity => entity.Entity.DomainEvents.ToList().Clear());

        return domainEvents;
    }
}
