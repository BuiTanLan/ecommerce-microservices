namespace BuildingBlocks.Core.Domain.Model;

public class AggregateId<T> : Identity<T>
{
    public AggregateId(){}
    public AggregateId(T value) : base(value)
    {
    }

    public static implicit operator T(AggregateId<T> id) => id.Value;
    public static implicit operator AggregateId<T>(T id) => new(id);
}


public class AggregateId : AggregateId<long>
{
    public AggregateId(){}
    public AggregateId(long value) : base(value)
    {
    }

    public static implicit operator long(AggregateId id) => id.Value;
    public static implicit operator AggregateId(long id) => new(id);
}
