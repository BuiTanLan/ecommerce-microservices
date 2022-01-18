namespace BuildingBlocks.Messaging.Message;

public interface IMessage
{
    Guid Id { get; set; }
    Guid CorrelationId { get; set; }
    DateTime OccurredOn { get; set; }
}
