using System;

namespace BuildingBlocks.Domain;

public abstract class Entity<TKey> : IEntity<TKey>
{
    public DateTime Created { get; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    public DateTime? Updated { get; protected set; }
    public TKey Id { get; init; }
}

public abstract class Entity : Entity<Guid>
{
}
