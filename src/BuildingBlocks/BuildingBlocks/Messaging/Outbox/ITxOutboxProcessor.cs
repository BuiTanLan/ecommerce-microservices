namespace BuildingBlocks.Messaging.Outbox;

public interface ITxOutboxProcessor
{
    Task HandleAsync(Type integrationAssemblyType, CancellationToken cancellationToken = default);
}
