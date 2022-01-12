using System;

namespace BuildingBlocks.Domain;

public interface IEntity : IEntity<Guid>
{
    public DateTime Created { get; }
    public DateTime? Updated { get; }
}

public interface IEntity<TId>
{
    public TId Id { get; init; }
}
