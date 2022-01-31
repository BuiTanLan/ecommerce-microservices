using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.Extensions.ServiceCollectionExtensions;

public static partial class ServiceCollectionExtensions
{
    public static void Unregister<TService>(this IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
        services.Remove(descriptor);
    }

    public static void Replace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
    {
        services.Unregister<TService>();
        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
    }

    /// <summary>
    ///     Adds a new transient registration to the service collection only when no existing registration of the same service
    ///     type and implementation type exists.
    ///     In contrast to TryAddTransient, which only checks the service type.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="serviceType">Service type</param>
    /// <param name="implementationType">Implementation type</param>
    private static void TryAddTransientExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType)) return;

        services.AddTransient(serviceType, implementationType);
    }

    /// <summary>
    ///     Adds a new transient registration to the service collection only when no existing registration of the same service
    ///     type and implementation type exists.
    ///     In contrast to TryAddScoped, which only checks the service type.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">Service type.</param>
    /// <param name="implementationType">Implementation type.</param>
    private static void TryAddScopeExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType)) return;

        services.AddScoped(serviceType, implementationType);
    }

    /// <summary>
    ///     Adds a new transient registration to the service collection only when no existing registration of the same service
    ///     type and implementation type exists.
    ///     In contrast to TryAddSingleton, which only checks the service type.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">Service type.</param>
    /// <param name="implementationType">Implementation type.</param>
    private static void TryAddSingletonExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType)) return;

        services.AddSingleton(serviceType, implementationType);
    }

    public static void ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.AddScoped<TService, TImplementation>();
    }

    public static void ReplaceScoped<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.Unregister<TService>();
        services.AddScoped(implementationFactory);
    }

    public static void ReplaceTransient<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.AddTransient<TService, TImplementation>();
    }

    public static void ReplaceTransient<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.Unregister<TService>();
        services.AddTransient(implementationFactory);
    }

    public static void ReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.AddSingleton<TService, TImplementation>();
    }

    public static void ReplaceSingleton<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.Unregister<TService>();
        services.AddSingleton(implementationFactory);
    }

    public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, new()
    {
        var options = new TOptions();
        configuration.Bind(typeof(TOptions).Name, options);

        services.AddSingleton(options);
    }

    public static void RegisterOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string name)
        where TOptions : class, new()
    {
        var options = new TOptions();
        configuration.Bind(name, options);

        services.AddSingleton(options);
    }
}
