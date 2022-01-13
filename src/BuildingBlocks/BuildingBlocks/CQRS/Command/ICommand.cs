using MediatR;

namespace BuildingBlocks.CQRS.Command;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out T> : IRequest<T>
    where T : notnull
{
}
