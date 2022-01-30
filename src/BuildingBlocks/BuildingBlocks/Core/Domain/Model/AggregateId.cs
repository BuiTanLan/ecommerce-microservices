using Ardalis.GuardClauses;

namespace BuildingBlocks.Core.Domain.Model;

public class AggregateId<T> : Identity<T>
{
    public AggregateId(T value) : base(value)
    {
    }

    public static implicit operator T(AggregateId<T> id) => Guard.Against.Null(id.Value, nameof(id.Value));
    public static implicit operator AggregateId<T>(T id) => new(id);
}

public class AggregateId : AggregateId<long>
{
    public AggregateId(long value) : base(value)
    {
        Guard.Against.NegativeOrZero(value, nameof(value));
    }

    public static implicit operator long(AggregateId id) => Guard.Against.Null(id.Value, nameof(id.Value));
    public static implicit operator AggregateId(long id) => new(id);
}
