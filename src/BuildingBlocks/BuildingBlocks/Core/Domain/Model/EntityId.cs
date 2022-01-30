using Ardalis.GuardClauses;

namespace BuildingBlocks.Core.Domain.Model;

public class EntityId : Identity
{
    public EntityId(long value) : base(value)
    {
        Guard.Against.NegativeOrZero(value, nameof(value));
    }

    public static implicit operator long(EntityId id) => Guard.Against.Null(id.Value, nameof(id.Value));
    public static implicit operator EntityId(long id) => new(id);
}

public class EntityId<T> : Identity<T>
{
    public EntityId(T id) : base(id)
    {
    }

    public static implicit operator T(EntityId<T> id) => Guard.Against.Null(id.Value, nameof(id.Value));
    public static implicit operator EntityId<T>(T id) => new(id);
}

