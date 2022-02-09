using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Persistence;

namespace BuildingBlocks.CQRS.Command;

public interface ICreateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{
}

public interface ICreateCommand : ICommand
{
}
