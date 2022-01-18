using System.Reflection;
using BuildingBlocks.Core.Objects;
using BuildingBlocks.Domain.Events.External;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var systemInfo = SystemInfo.New();
        var typeResolver = new TypeResolver();

        services.AddSingleton<ITypeResolver>(typeResolver);
        services.AddSingleton<ISystemInfo>(systemInfo);
        services.AddSingleton(systemInfo);

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
