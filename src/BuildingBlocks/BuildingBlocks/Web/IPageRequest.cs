using BuildingBlocks.CQRS;

namespace BuildingBlocks.Web;

public interface IPageRequest
{
    IList<string> Includes { get; set; }
    IList<FilterModel> Filters { get; set; }
    IList<string> Sorts { get; set; }
    int Page { get; set; }
    int PageSize { get; set; }
}
