using System;
using System.Collections.Generic;
using System.Reflection;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching.Redis;
using BuildingBlocks.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BuildingBlocks.Caching;

public static class Extensions
{
    public static IServiceCollection AddCachingRequestPolicies(this IServiceCollection services,
        IList<Assembly> assembliesToScan)
    {
        // ICachePolicy discovery and registration
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan ?? AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(ICachePolicy<,>)),
                false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        // IInvalidateCachePolicy discovery and registration
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan ?? AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IInvalidateCachePolicy<,>)),
                false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IServiceCollection AddCustomRedisCache(
        this IServiceCollection services,
        IConfiguration config,
        Action<RedisOptions> setupAction = null)
    {
        Guard.Against.Null(services, nameof(services));

        var redisOptions = new RedisOptions();
        var redisSection = config.GetSection(nameof(RedisOptions));

        redisSection.Bind(redisOptions);
        services.Configure<RedisOptions>(redisSection);
        setupAction?.Invoke(redisOptions);

        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = config[redisOptions.Prefix];
            options.ConfigurationOptions = GetRedisConfigurationOptions(redisOptions);
        });

        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }

    private static ConfigurationOptions GetRedisConfigurationOptions(RedisOptions redisOptions)
    {
        var configurationOptions = new ConfigurationOptions
        {
            ConnectTimeout = redisOptions.ConnectTimeout,
            SyncTimeout = redisOptions.SyncTimeout,
            ConnectRetry = redisOptions.ConnectRetry,
            AbortOnConnectFail = redisOptions.AbortOnConnectFail,
            ReconnectRetryPolicy = new ExponentialRetry(redisOptions.DeltaBackOffMillisecond),
            KeepAlive = 5,
            Ssl = redisOptions.Ssl
        };

        if (!string.IsNullOrWhiteSpace(redisOptions.Password))
        {
            configurationOptions.Password = redisOptions.Password;
        }

        var endpoints = redisOptions.Url.Split(',');
        foreach (var endpoint in endpoints)
        {
            configurationOptions.EndPoints.Add(endpoint);
        }

        return configurationOptions;
    }
}
