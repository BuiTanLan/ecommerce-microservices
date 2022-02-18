using BuildingBlocks.Core.Persistence;

namespace BuildingBlocks.CQRS.Command;

public interface ITxInternalCommand : IInternalCommand, ITxRequest
{
}
