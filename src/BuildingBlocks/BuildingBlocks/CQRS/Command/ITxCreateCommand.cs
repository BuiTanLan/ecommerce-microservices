using BuildingBlocks.Core.Persistence;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public interface ITxCreateCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
{
}

public interface ITxCreateCommand : ITxCreateCommand<Unit>
{
}
