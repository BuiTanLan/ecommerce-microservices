using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Messaging.Outbox;

public class EfOutboxOptions
{
    public string ConnectionString { get; set; }
}

public static class EfOutboxOptionsExtensions
{
    public static EfOutboxOptions GetEfOutboxOptions(this IConfiguration configuration)
    {
        return configuration.GetSection(nameof(EfOutboxOptions)).Get<EfOutboxOptions>();
    }
}
