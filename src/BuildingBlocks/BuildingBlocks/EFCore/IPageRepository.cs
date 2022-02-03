using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.EFCore.Specification;

namespace BuildingBlocks.EFCore;

public interface IPageRepository<TEntity, Tkey>
    where TEntity : IAggregateRoot<Tkey>
{
    ValueTask<long> CountAsync(IPageSpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<List<TEntity>> FindAsync(IPageSpecification<TEntity> spec, CancellationToken cancellationToken = default);
}

public interface IPageRepository<TEntity> : IPageRepository<TEntity, Guid>
    where TEntity : IAggregateRoot<Guid>
{
}
