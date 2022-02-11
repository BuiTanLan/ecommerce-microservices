using BuildingBlocks.CQRS.Command;

namespace BuildingBlocks.Scheduling.Internal.Services;

public interface IInternalMessageService
{
    Task<IEnumerable<InternalMessage>> GetAllUnsentInternalMessagesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InternalMessage>> GetAllInternalMessagesAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(IInternalCommand internalCommand, CancellationToken cancellationToken = default);
    Task PublishUnsentInternalMessagesAsync(CancellationToken cancellationToken = default);
}
