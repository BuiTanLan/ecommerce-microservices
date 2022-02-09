using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Persistence;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public interface IUpdateCommand : IUpdateCommand<Unit>
{
}

public interface IUpdateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{
}
