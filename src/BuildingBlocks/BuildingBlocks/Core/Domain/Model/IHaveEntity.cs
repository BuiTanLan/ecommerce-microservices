namespace BuildingBlocks.Core.Domain.Model;

public interface IHaveEntity
{
    DateTime Created { get; }
    int? CreatedBy { get; }
}
