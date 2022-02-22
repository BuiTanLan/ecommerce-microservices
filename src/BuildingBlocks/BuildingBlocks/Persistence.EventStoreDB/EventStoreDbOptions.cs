using BuildingBlocks.Persistence.EventStoreDB.Subscriptions;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Persistence.EventStoreDB;

public class EventStoreDbOptions
{
    public bool UseInternalCheckpointing { get; set; } = true;
    public string ConnectionString { get; set; } = default!;
}
