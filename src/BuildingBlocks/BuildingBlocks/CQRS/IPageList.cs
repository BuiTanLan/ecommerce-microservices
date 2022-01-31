namespace BuildingBlocks.CQRS;

public interface IPageList
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public IList<string>? Includes { get; init; }
    public IList<FilterModel>? Filters { get; init; }
    public IList<string>? Sorts { get; init; }
}
