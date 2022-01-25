namespace BuildingBlocks.Core.Domain.Model;

public interface IAuditableEntity<TId> : IEntity<TId>, IHaveAudit
{
}

public interface IAuditableEntity<TIdentity, TId> : IAuditableEntity<TIdentity>
    where TIdentity : Identity<TId>
{
}

public interface IAuditableEntity : IAuditableEntity<Identity<long>, long>
{
}
