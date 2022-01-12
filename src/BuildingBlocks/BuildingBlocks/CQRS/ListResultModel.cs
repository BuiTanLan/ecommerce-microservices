using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.CQRS;

public record ListResultModel<T>(List<T> Items, long TotalItems, int Page, int PageSize) where T : notnull
{
    public static ListResultModel<T> Create(List<T> items, long totalItems = 0, int page = 1, int pageSize = 20)
    {
        return new(items, totalItems, page, pageSize);
    }

    public ListResultModel<U> Map<U>(Func<T, U> map) => ListResultModel<U>.Create(
        this.Items.Select<T, U>(map).ToList(),
        this.TotalItems, this.Page, this.PageSize);

    public static ListResultModel<T> Empty => new(Enumerable.Empty<T>().ToList(), 0, 0, 0);
}
