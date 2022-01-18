namespace BuildingBlocks.Core.Objects;

public interface ISystemInfo
{
    string ClientGroup { get; }
    Guid ClientId { get; }
    bool PublishOnly { get; }
}
