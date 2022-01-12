using MediatR;

namespace BuildingBlocks.CQRS.Command;

public interface ICommand : IRequest
{
}

public interface ICommand<out T> : IRequest<T> where T : notnull
{
}
