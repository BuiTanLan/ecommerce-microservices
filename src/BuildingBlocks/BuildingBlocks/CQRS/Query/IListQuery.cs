namespace BuildingBlocks.CQRS.Query;

public interface IListQuery<out TResponse> : IQuery<TResponse>, IPageList
    where TResponse : notnull
{
}
