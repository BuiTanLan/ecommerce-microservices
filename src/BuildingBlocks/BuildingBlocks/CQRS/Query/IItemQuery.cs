using System.Collections.Generic;

namespace BuildingBlocks.CQRS.Query
{
    public interface IItemQuery<TId, out TResponse> : IQuery<TResponse>
        where TId : struct
        where TResponse : notnull
    {
        public List<string> Includes { get; }
        public TId Id { get; }
    }
}
