namespace BuildingBlocks.Core.Domain.Model;

public interface IHaveAudit
{
    DateTime? LastModified { get; }
    int? LastModifiedBy { get; set; }
}
