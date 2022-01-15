using System.Reflection;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Model;
using Newtonsoft.Json;

namespace BuildingBlocks.Messaging.Outbox;

public class OutboxMessage : AggregateRoot
{
    /// <summary>
    /// Gets or sets name of message
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the date the message occurred.
    /// </summary>
    public DateTime OccurredOn { get; init; }

    /// <summary>
    /// Gets the event type full name.
    /// </summary>
    public string Type { get; init; }

    /// <summary>
    /// Gets the event data - serialized to JSON.
    /// </summary>
    public string Data { get; init; }

    /// <summary>
    /// Gets the date the message processed.
    /// </summary>
    public DateTime? ProcessedOn { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMessage"/> class.
    /// Initializes a new outbox message.
    /// </summary>
    /// <param name="id">The outbox message identifier.</param>
    /// <param name="occurredOn">The outbox message date occurred on.</param>
    /// <param name="type">The outbox message type.</param>
    /// <param name="data">The outbox message data.</param>
    public OutboxMessage(Guid id, DateTime occurredOn, string type, string data)
    {
        OccurredOn = occurredOn;
        Type = type;
        Data = data;
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxMessage"/> class.
    /// Initializes a new outbox message.
    /// </summary>
    /// <param name="id">The outbox message identifier.</param>
    /// <param name="occurredOn">The outbox message date occurred on.</param>
    /// <param name="event">Our domain event</param>
    public OutboxMessage(Guid id, DateTime occurredOn, IDomainEvent @event)
    {
        OccurredOn = occurredOn;
        Type = Type = @event.GetType().FullName;
        Data = JsonConvert.SerializeObject(@event);
        Id = id;
    }

    /// <summary>
    /// Sets outbox message process date.
    /// </summary>
    public void ChangeProcessDate()
    {
        ProcessedOn = DateTime.Now;
    }

    public virtual IDomainEvent RecreateMessage(Assembly assembly) =>
        (IDomainEvent)JsonConvert.DeserializeObject(Data, assembly.GetType(Type)!);
}
