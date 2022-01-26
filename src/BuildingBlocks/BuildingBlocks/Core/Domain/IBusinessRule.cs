namespace BuildingBlocks.Core.Domain;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
