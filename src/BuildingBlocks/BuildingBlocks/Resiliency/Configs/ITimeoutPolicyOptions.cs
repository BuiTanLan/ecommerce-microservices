namespace BuildingBlocks.Resiliency.Configs;

public interface ITimeoutPolicyOptions
{
    public int TimeOutDuration { get; set; }
}
