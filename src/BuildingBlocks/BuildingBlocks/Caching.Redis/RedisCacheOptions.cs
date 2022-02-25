using BuildingBlocks.Abstractions.Caching;

namespace BuildingBlocks.Caching.Redis;

public class RedisCacheOptions : CacheOptions
{
    public string? ConnectionString { get; set; }
    public int Db { get; set; } = -1;
    public object? AsyncState { get; set; } = null;
    public int RetryCount { get; set; } = 3;
}
