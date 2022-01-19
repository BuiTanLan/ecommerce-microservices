using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.CQRS;
using BuildingBlocks.EFCore.Specification;
using Microsoft.EntityFrameworkCore;
using FilterModel = BuildingBlocks.CQRS.FilterModel;

namespace BuildingBlocks.EFCore;

public static class QueryableExtensions
{
    public static async Task<ListResultModel<T>> PaginateAsync<T>(
        this IQueryable<T> collection,
        IPageList query)
    {
        return await collection.PaginateAsync(query.Page, query.PageSize);
    }

    public static async Task<ListResultModel<T>> PaginateAsync<T>(this IQueryable<T> collection, int page = 1, int
        pageSize = 10)
    {
        if (page <= 0) page = 1;

        if (pageSize <= 0) pageSize = 10;

        var isEmpty = await collection.AnyAsync() == false;
        if (isEmpty) return ListResultModel<T>.Empty;

        var totalItems = await collection.CountAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
        var data = await collection.Limit(page, pageSize).ToListAsync();

        return ListResultModel<T>.Create(data, totalItems, page, pageSize);
    }

    public static async Task<ListResultModel<R>> PaginateAsync<T,R>(
        this IQueryable<T> collection,
        IConfigurationProvider configuration,
        int page = 1,
        int pageSize = 10)
    {
        if (page <= 0) page = 1;

        if (pageSize <= 0) pageSize = 10;

        var isEmpty = await collection.AnyAsync() == false;
        if (isEmpty) return ListResultModel<R>.Empty;

        var totalItems = await collection.CountAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
        var data = await collection.Limit(page, pageSize).ProjectTo<R>(configuration).ToListAsync();

        return ListResultModel<R>.Create(data, totalItems, page, pageSize);
    }

    public static IQueryable<T> Limit<T>(this IQueryable<T> collection, IPageList query)
    {
        return collection.Limit(query.Page, query.PageSize);
    }

    public static IQueryable<T> Limit<T>(
        this IQueryable<T> collection,
        int page = 1,
        int resultsPerPage = 10)
    {
        if (page <= 0) page = 1;

        if (resultsPerPage <= 0) resultsPerPage = 10;

        var skip = (page - 1) * resultsPerPage;
        var data = collection.Skip(skip)
            .Take(resultsPerPage);

        return data;
    }

    public static IQueryable<TEntity> ApplyIncludeList<TEntity>(
        this IQueryable<TEntity> source,
        IEnumerable<string> navigationPropertiesPath)
        where TEntity : class
    {
        if (navigationPropertiesPath is null)
            return source;

        foreach (var navigationPropertyPath in navigationPropertiesPath)
        {
            source = source.Include(navigationPropertyPath);
        }

        return source;
    }

    public static IQueryable<TEntity> ApplyFilterList<TEntity>(
        this IQueryable<TEntity> source,
        IEnumerable<FilterModel> filters)
        where TEntity : class
    {
        if (filters is null)
            return source;

        List<Expression<Func<TEntity, bool>>> filterExpressions = new List<Expression<Func<TEntity, bool>>>();

        foreach (var (fieldName, comparision, fieldValue) in filters)
        {
            Expression<Func<TEntity, bool>> expr = PredicateBuilder.Build<TEntity>(fieldName, comparision, fieldValue);
            filterExpressions.Add(expr);
        }

        return source.Where(filterExpressions.Aggregate((expr1, expr2) => expr1.And(expr2)));
    }

    public static IQueryable<TEntity> ApplyPaging<TEntity>(
        this IQueryable<TEntity> source,
        int page,
        int size)
        where TEntity : class
    {
        return source.Skip(page * size).Take(size);
    }
}
