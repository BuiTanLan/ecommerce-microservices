namespace BuildingBlocks.Core.Domain.Model;

public interface IHaveAudit : IHaveEntity
{
    DateTime? LastModified { get; }
    int? LastModifiedBy { get; }
}
