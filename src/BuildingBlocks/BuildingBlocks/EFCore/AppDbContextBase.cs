using System.Collections.Immutable;
using System.Data;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.Internal;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EFCore;

public abstract class AppDbContextBase :
    DbContext,
    IDomainEventContext,
    IDbFacadeResolver,
    IDbContext
{
    private readonly IMediator _mediator;
    private IDbContextTransaction _currentTransaction;

    protected AppDbContextBase(DbContextOptions options) : base(options)
    {
    }

    protected AppDbContextBase(DbContextOptions options, IMediator mediator) : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
        // https://ardalis.com/immediate-domain-event-salvation-with-mediatr/
        await _mediator.DispatchDomainEventsAsync(GetDomainEvents());

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
        // performed through the DbContext will be committed
        var result = await SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task RetryOnExceptionAsync(Func<Task> operation)
    {
        await Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public async Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
    {
        return await Database.CreateExecutionStrategy().ExecuteAsync(operation);
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregateRoot<long>>()
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
