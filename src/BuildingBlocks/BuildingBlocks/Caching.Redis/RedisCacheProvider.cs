using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Abstractions.Messaging.Serialization;
using Microsoft.Extensions.Options;
using Polly;
using StackExchange.Redis;

namespace BuildingBlocks.Caching.Redis;

public class RedisCacheProvider : ICacheProvider
{
    private readonly IMessageSerializer _messageSerializer;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly RedisCacheOptions _cacheOptions;

    private IDatabase Database => _connectionMultiplexer.GetDatabase(_cacheOptions.Db, _cacheOptions.AsyncState);

    public RedisCacheProvider(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisCacheOptions> cacheOptions,
        IMessageSerializer messageSerializer)
    {
        _messageSerializer = Guard.Against.Null(messageSerializer, nameof(messageSerializer));
        _connectionMultiplexer = Guard.Against.Null(connectionMultiplexer, nameof(connectionMultiplexer));
        _cacheOptions = Guard.Against.Null(cacheOptions.Value, nameof(cacheOptions));
    }


    private IAsyncPolicy RetryPolicyAsync =>
        Policy
            .Handle<System.Exception>()
            .WaitAndRetryAsync(
                _cacheOptions.RetryCount, // number of retries
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential backoff
            );

    private Policy RetryPolicy =>
        Policy
            .Handle<System.Exception>()
            .WaitAndRetry(
                _cacheOptions.RetryCount, // number of retries
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential backoff
            );

    public async Task<T?> GetAsync<T>(string key)
    {
        Guard.Against.NullOrEmpty(key, nameof(key));

        var value = await RetryPolicyAsync.ExecuteAsync(() => Database.StringGetAsync(key));
        return value.IsNullOrEmpty ? default : _messageSerializer.Deserialize<T>(value);
    }

    public void Set(string key, object data, int? cacheTime = null)
    {
        Guard.Against.NullOrEmpty(key, nameof(key));
        Guard.Against.Null(data, nameof(data));

        var json = _messageSerializer.Serialize(data);
        var time = TimeSpan.FromSeconds(cacheTime ?? _cacheOptions.DefaultCacheTime);

        RetryPolicy.Execute(() => Database.StringSet(key, json, time));
    }

    public Task<bool> IsSetAsync(string key)
    {
        return RetryPolicyAsync.ExecuteAsync(() => Database.KeyExistsAsync(key));
    }

    public Task RemoveAsync(string key)
    {
        Guard.Against.NullOrEmpty(key, nameof(key));

        return RetryPolicyAsync.ExecuteAsync(() => Database.KeyDeleteAsync(key));
    }

    public T? Get<T>(string key)
    {
        Guard.Against.NullOrEmpty(key, nameof(key));

        var value = RetryPolicy.Execute(() => Database.StringGet(key));
        return value.IsNullOrEmpty ? default : _messageSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync(string key, object data, int? cacheTime = null)
    {
        Guard.Against.NullOrEmpty(key, nameof(key));
        Guard.Against.Null(data, nameof(data));

        var json = _messageSerializer.Serialize(data);
        await RetryPolicyAsync.ExecuteAsync(() =>
            Database.StringSetAsync(key, json, TimeSpan.FromSeconds(cacheTime ?? _cacheOptions.DefaultCacheTime)));
    }

    public bool IsSet(string key)
    {
        return RetryPolicy.Execute(() => Database.KeyExists(key));
    }

    public void Remove(string key)
    {
        Guard.Against.NullOrEmpty(key, nameof(key));

        RetryPolicy.Execute(() => Database.KeyDelete(key));
    }
}
