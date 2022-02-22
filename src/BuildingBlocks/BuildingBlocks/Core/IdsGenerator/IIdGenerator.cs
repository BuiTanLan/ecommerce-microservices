namespace BuildingBlocks.Core.IdsGenerator;

public interface IIdGenerator<out TId>
{
    TId New();
}
