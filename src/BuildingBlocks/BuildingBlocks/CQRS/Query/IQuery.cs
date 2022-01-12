using MediatR;

namespace BuildingBlocks.CQRS.Query;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}
