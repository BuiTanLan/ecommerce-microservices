namespace BuildingBlocks.Abstractions.Domain.Model;

public interface IReadEntity<TId>
{
    public TId Id { get; set; }
}

public interface IReadEntity : IReadEntity<long>
{
}
