using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Messaging.Outbox;

public class OutboxOptions
{
    public string ConnectionString { get; set; }
    public bool Enabled { get; set; } = true;
    public TimeSpan? Interval { get; set; }
    public bool UseBackgroundDispatcher { get; set; } = true;
}

public static class OutboxOptionsExtensions
{
    public static OutboxOptions GetOutboxOptions(this IConfiguration configuration)
    {
        return configuration.GetSection(nameof(OutboxOptions)).Get<OutboxOptions>();
    }
}
