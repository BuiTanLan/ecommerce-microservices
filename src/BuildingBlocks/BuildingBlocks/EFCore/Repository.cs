using BuildingBlocks.Core.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EFCore;

public class Repository<TDbContext, TEntity, TKey> : RepositoryBase<TDbContext, TEntity, TKey>
    where TEntity : class, IAggregateRoot<TKey>
    where TDbContext : DbContext
{
    public Repository(TDbContext dbContext) : base(dbContext)
    {
    }
}

public class Repository<TDbContext, TEntity> : Repository<TDbContext, TEntity, Guid>
    where TEntity : class, IAggregateRoot<Guid>
    where TDbContext : DbContext
{
    public Repository(TDbContext dbContext) : base(dbContext)
    {
    }
}
