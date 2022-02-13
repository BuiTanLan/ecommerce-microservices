namespace BuildingBlocks.Resiliency.Configs;

public interface ICircuitBreakerPolicyOptions
{
    int RetryCount { get; set; }
    int BreakDuration { get; set; }
}
