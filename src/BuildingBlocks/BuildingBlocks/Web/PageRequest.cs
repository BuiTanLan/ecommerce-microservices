using BuildingBlocks.CQRS;

namespace BuildingBlocks.Web;

public record PageRequest(
    int Page,
    int PageSize,
    IList<string>? Includes = null,
    IList<FilterModel>? Filters = null,
    IList<string>? Sorts = null) : IPageRequest;
