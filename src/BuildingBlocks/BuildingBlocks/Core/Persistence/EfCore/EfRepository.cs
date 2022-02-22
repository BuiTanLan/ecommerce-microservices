using BuildingBlocks.Abstractions.Domain.Events;
using BuildingBlocks.Abstractions.Domain.Model;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Persistence.EfCore;

public class EfRepository<TDbContext, TEntity, TKey> : EfRepositoryBase<TDbContext, TEntity, TKey>
    where TEntity : class, IHaveIdentity<TKey>
    where TDbContext : DbContext
{
    public EfRepository(TDbContext dbContext, IAggregatesDomainEventsStore aggregatesDomainEventsStore)
        : base(dbContext, aggregatesDomainEventsStore)
    {
    }
}

public class EfRepository<TDbContext, TEntity> : EfRepository<TDbContext, TEntity, Guid>
    where TEntity : class, IHaveIdentity<Guid>
    where TDbContext : DbContext
{
    public EfRepository(TDbContext dbContext, IAggregatesDomainEventsStore aggregatesDomainEventsStore)
        : base(dbContext, aggregatesDomainEventsStore)
    {
    }
}
