namespace BuildingBlocks.Domain;

public interface IAggregateRoot : IAggregateRoot<int>
{
}

public interface IAggregateRoot<TId>
{
    public TId Id { get; init; }
}
