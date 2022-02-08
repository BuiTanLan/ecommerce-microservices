using BuildingBlocks.Core.Domain;
using MediatR;

namespace BuildingBlocks.CQRS.Command;

public interface ITxCommand : ITxCommand<Unit>
{
}

public interface ITxCommand<out T> : ICommand<T>, ITxRequest
    where T : notnull
{
}
