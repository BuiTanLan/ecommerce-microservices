using System;
using System.Linq.Expressions;

namespace BuildingBlocks.EFCore.Specification;

public class NoOpSpec<TEntity> : SpecificationBase<TEntity>
{
    public override Expression<Func<TEntity, bool>> Criteria => p => true;
}
