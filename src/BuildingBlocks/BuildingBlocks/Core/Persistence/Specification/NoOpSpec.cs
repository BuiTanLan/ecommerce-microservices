using System.Linq.Expressions;

namespace BuildingBlocks.Core.Persistence.Specification;

public class NoOpSpec<TEntity> : SpecificationBase<TEntity>
{
    public override Expression<Func<TEntity, bool>> Criteria => p => true;
}
