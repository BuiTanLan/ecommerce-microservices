using BuildingBlocks.Domain;

namespace BuildingBlocks.CQRS.Command;

public interface IUpdateCommand : ICommand, ITxRequest
{
}

public interface IUpdateCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
{
}
