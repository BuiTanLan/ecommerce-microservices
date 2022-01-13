using BuildingBlocks.Domain;
using BuildingBlocks.EFCore.Specification;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EFCore;

public abstract class RepositoryBase<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>,
    IPageRepository<TEntity, TKey>
    where TEntity : class, IAggregateRoot<TKey>
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    protected RepositoryBase(TDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public async ValueTask<long> CountAsync(
        IPageSpecification<TEntity> spec,
        CancellationToken cancellationToken = default)
    {
        spec.IsPagingEnabled = false;
        var specificationResult = GetQuery(DbSet, spec);
        return await ValueTask.FromResult(
            await specificationResult.LongCountAsync(cancellationToken));
    }

    public async Task<List<TEntity>> FindAsync(
        IPageSpecification<TEntity> spec,
        CancellationToken cancellationToken = default)
    {
        var specificationResult = GetQuery(DbSet, spec);
        return await specificationResult.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public async Task<TEntity> FindOneAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var specificationResult = GetQuery(DbSet, spec);
        return await specificationResult.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync(
        ISpecification<TEntity> spec,
        CancellationToken cancellationToken = default)
    {
        var specificationResult = GetQuery(DbSet, spec);
        return await specificationResult.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        if (autoSave) await DbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TEntity> EditAsync(TEntity entity, bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var entry = DbContext.Entry(entity);
        entry.State = EntityState.Modified;

        if (autoSave) await DbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(entry.Entity);
    }

    public async ValueTask DeleteAsync(TEntity entity, bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);

        if (autoSave) await DbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.Criteria is not null) query = query.Where(specification.Criteria);

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.GroupBy is not null)
        {
            query = query
                .GroupBy(specification.GroupBy)
                .SelectMany(x => x);
        }

        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(specification.Skip - 1)
                .Take(specification.Take);
        }

        query = query.AsSplitQuery();

        return query;
    }

    private static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
        IPageSpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.Criterias is not null && specification.Criterias.Count > 0)
        {
            var expr = specification.Criterias.First();
            for (var i = 1; i < specification.Criterias.Count; i++) expr = expr.And(specification.Criterias[i]);

            query = query.Where(expr);
        }

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.GroupBy is not null)
        {
            query = query
                .GroupBy(specification.GroupBy)
                .SelectMany(x => x);
        }

        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(specification.Skip - 1)
                .Take(specification.Take);
        }

        query = query.AsSplitQuery();

        return query;
    }
}

public class Repository<TDbContext, TEntity> : RepositoryBase<TDbContext, TEntity, Guid>
    where TEntity : class, IAggregateRoot<Guid>
    where TDbContext : DbContext
{
    public Repository(TDbContext dbContext) : base(dbContext)
    {
    }
}
