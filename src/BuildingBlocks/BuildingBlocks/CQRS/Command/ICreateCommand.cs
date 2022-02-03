using BuildingBlocks.Core.Domain;

namespace BuildingBlocks.CQRS.Command;

public interface ICreateCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
{
}

public interface ICreateCommand : ICommand, ITxRequest
{
}
