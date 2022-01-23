using System.Reflection;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Events.External;
using BuildingBlocks.Core.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var systemInfo = SystemInfo.New();
        var typeResolver = new TypeResolver();

        services.AddSingleton<ITypeResolver>(typeResolver);
        services.AddSingleton<ISystemInfo>(systemInfo);
        services.AddSingleton(systemInfo);
        services.AddTransient<IEventProcessor, EventProcessor>();

        RegisterAllMessages(typeResolver);

        return services;
    }

    /// <summary>
    /// caches all the message types. This will allow all the classes referencing ITypeResolver to work properly.
    /// </summary>
    private static void RegisterAllMessages(ITypeResolver typeResolver)
    {
        Console.WriteLine("preloading all message types...");

        var messageType = typeof(IIntegrationEvent);

        // Assemblies are lazy loaded so using AppDomain.GetAssemblies is not reliable.
        var currAssembly = Assembly.GetEntryAssembly();
        var visited = new HashSet<string>();
        var queue = new Queue<Assembly>();
        queue.Enqueue(currAssembly);
        while (queue.Any())
        {
            var assembly = queue.Dequeue();
            visited.Add(assembly.FullName);

            var assemblyTypes = assembly.GetTypes();
            foreach (var type in assemblyTypes)
            {
                if (messageType.IsAssignableFrom(type))
                    typeResolver.Register(type);
            }

            var references = assembly.GetReferencedAssemblies();
            foreach (var reference in references)
            {
                if (visited.Contains(reference.FullName))
                    continue;
                queue.Enqueue(Assembly.Load(reference));
            }
        }

        Console.WriteLine("preloading all message types completed!");
    }
}
