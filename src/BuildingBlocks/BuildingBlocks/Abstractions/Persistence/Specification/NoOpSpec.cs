using System.Linq.Expressions;

namespace BuildingBlocks.Abstractions.Persistence.Specification;

public class NoOpSpec<TEntity> : SpecificationBase<TEntity>
{
    public override Expression<Func<TEntity, bool>> Criteria => p => true;
}
