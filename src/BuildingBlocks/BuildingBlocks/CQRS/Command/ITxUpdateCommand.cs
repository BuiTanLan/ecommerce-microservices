using BuildingBlocks.Core.Persistence;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public interface ITxUpdateCommand<out TResponse> : IUpdateCommand<TResponse>, ITxRequest
    where TResponse : notnull
{
}

public interface ITxUpdateCommand : ITxUpdateCommand<Unit>
{
}
