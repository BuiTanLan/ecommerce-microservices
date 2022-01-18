using MediatR;

namespace BuildingBlocks.Domain.Events.External;

/// <summary>
/// The integration event interface.
/// </summary>
public interface IIntegrationEvent : IEvent, INotification
{
    public string CorrelationId { get; }
}
