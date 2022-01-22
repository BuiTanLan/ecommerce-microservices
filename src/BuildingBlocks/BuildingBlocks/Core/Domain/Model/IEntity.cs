namespace BuildingBlocks.Core.Domain.Model;

public interface IEntity<out TId> : IIdentity<TId>
{
    DateTime Created { get; }
    int? CreatedBy { get; set; }
}

public interface IEntity : IEntity<Guid>
{
}

