using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Caching;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly IEnumerable<ICachePolicy<TRequest, TResponse>> _cachePolicies;
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private readonly int defaultCacheExpirationInHours = 1;

    public CachingBehavior(
        IEasyCachingProviderFactory cachingFactory,
        ILogger<CachingBehavior<TRequest, TResponse>> logger,
        IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies)
    {
        _logger = logger;
        _cachingProvider = cachingFactory.GetCachingProvider("mem");
        _cachePolicies = cachePolicies;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var cachePolicy = _cachePolicies.FirstOrDefault();
        if (cachePolicy == null)
        {
            // No cache policy found, so just continue through the pipeline
            return await next();
        }

        var cacheKey = cachePolicy.GetCacheKey(request);
        var cachedResponse = await _cachingProvider.GetAsync<TResponse>(cacheKey);
        if (cachedResponse.Value != null)
        {
            _logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey);
            return cachedResponse.Value;
        }

        var response = await next();

        var time = cachePolicy.AbsoluteExpirationRelativeToNow ??
                   DateTime.Now.AddHours(defaultCacheExpirationInHours);
        await _cachingProvider.SetAsync(cacheKey, response, time.TimeOfDay);

        _logger.LogDebug(
            "Caching response for {TRequest} with cache key: {CacheKey}",
            typeof(TRequest).FullName,
            cacheKey);

        return response;
    }
}

public class StreamCachingBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IStreamRequest<TResponse>
    where TResponse : notnull
{
    private readonly IEnumerable<IStreamCachePolicy<TRequest, TResponse>> _cachePolicies;
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly ILogger<StreamCachingBehavior<TRequest, TResponse>> _logger;
    private readonly int defaultCacheExpirationInHours = 1;

    public StreamCachingBehavior(
        IEasyCachingProviderFactory cachingFactory,
        ILogger<StreamCachingBehavior<TRequest, TResponse>> logger,
        IEnumerable<IStreamCachePolicy<TRequest, TResponse>> cachePolicies)
    {
        _logger = logger;
        _cachingProvider = cachingFactory.GetCachingProvider("mem");
        _cachePolicies = cachePolicies;
    }

    public IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        StreamHandlerDelegate<TResponse> next)
    {
        var cachePolicy = _cachePolicies.FirstOrDefault();
        if (cachePolicy == null)
        {
            // No cache policy found, so just continue through the pipeline
            return next();
        }

        var cacheKey = cachePolicy.GetCacheKey(request);
        var cachedResponse = _cachingProvider.Get<IAsyncEnumerable<TResponse>>(cacheKey);
        if (cachedResponse.Value != null)
        {
            _logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey);
            return cachedResponse.Value;
        }

        var response = next();

        var time = cachePolicy.AbsoluteExpirationRelativeToNow ??
                   DateTime.Now.AddHours(defaultCacheExpirationInHours);
        _cachingProvider.Set(cacheKey, response, time.TimeOfDay);

        _logger.LogDebug(
            "Caching response for {TRequest} with cache key: {CacheKey}",
            typeof(TRequest).FullName,
            cacheKey);

        return response;
    }
}
