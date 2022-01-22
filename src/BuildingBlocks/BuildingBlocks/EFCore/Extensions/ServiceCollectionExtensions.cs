using System.Reflection;
using BuildingBlocks.Core.Domain.Events;
using BuildingBlocks.Core.Domain.Model;
using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BuildingBlocks.EFCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDbContext<TDbContext>(
        this IServiceCollection services,
        string connString,
        Action<DbContextOptionsBuilder> doMoreDbContextOptionsConfigure = null,
        Action<IServiceCollection> doMoreActions = null)
        where TDbContext : AppDbContextBase, IDbFacadeResolver, IDomainEventContext
    {
        services.AddDbContext<TDbContext>(options =>
        {
            options.UseNpgsql(connString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(TDbContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();

            doMoreDbContextOptionsConfigure?.Invoke(options);
        });

        services.AddScoped<IDbContext>(provider => provider.GetService<TDbContext>());
        services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<TDbContext>());
        services.AddScoped<IDomainEventContext>(provider => provider.GetService<TDbContext>());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TxBehavior<,>));

        doMoreActions?.Invoke(services);

        return services;
    }

    public static IServiceCollection AddCustomRepository(this IServiceCollection services, Type customRepositoryType)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(customRepositoryType)
            .AddClasses(classes =>
                classes.AssignableTo(customRepositoryType)).As(typeof(IRepository<,>)).WithScopedLifetime()
            .AddClasses(classes =>
                classes.AssignableTo(customRepositoryType)).As(typeof(IPageRepository<>)).WithScopedLifetime()
        );

        return services;
    }

    public static IServiceCollection AddCustomRepository<TEntity, TKey, TRepository>(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped)
        where TEntity : class, IAggregateRoot<TKey>
        where TRepository : class, IRepository<TEntity, TKey>
    {
        return services.RegisterService<IRepository<TEntity, TKey>, TRepository>(lifeTime);
    }

    public static IServiceCollection AddUnitOfWork<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped,
        bool registerGeneric = false)
        where TContext : AppDbContextBase
    {
        if (registerGeneric)
        {
            services.RegisterService<IUnitOfWork, EfUnitOfWork<TContext>>(lifeTime);
        }

        return services.RegisterService<IUnitOfWork<TContext>, EfUnitOfWork<TContext>>(lifeTime);
    }


    public static void MigrateDataFromScript(this MigrationBuilder migrationBuilder)
    {
        var assembly = Assembly.GetCallingAssembly();
        var files = assembly.GetManifestResourceNames();
        var filePrefix = $"{assembly.GetName().Name}.Data.Scripts.";

        foreach (var file in files
                     .Where(f => f.StartsWith(filePrefix) && f.EndsWith(".sql"))
                     .Select(f => new { PhysicalFile = f, LogicalFile = f.Replace(filePrefix, string.Empty) })
                     .OrderBy(f => f.LogicalFile))
        {
            using var stream = assembly.GetManifestResourceStream(file.PhysicalFile);
            using var reader = new StreamReader(stream!);
            var command = reader.ReadToEnd();

            if (string.IsNullOrWhiteSpace(command))
                continue;

            migrationBuilder.Sql(command);
        }
    }

    public static async Task DoDbMigrationAsync(this IApplicationBuilder app, ILogger logger)
    {
        var scope = app.ApplicationServices.CreateAsyncScope();
        var dbFacadeResolver = scope.ServiceProvider.GetService<IDbFacadeResolver>();

        var policy = CreatePolicy(3, logger, nameof(WebApplication));
        await policy.ExecuteAsync(async () =>
        {
            if (!await dbFacadeResolver?.Database.CanConnectAsync()!)
            {
                Console.WriteLine($"Connection String: {dbFacadeResolver?.Database.GetConnectionString()}");
                throw new System.Exception("Couldn't connect database.");
            }

            var migrations = await dbFacadeResolver?.Database.GetPendingMigrationsAsync()!;
            if (migrations.Any())
            {
                await dbFacadeResolver?.Database.MigrateAsync()!;
                logger?.LogInformation("Migration database schema. Done!!!");
            }
        });

        static AsyncRetryPolicy CreatePolicy(int retries, ILogger logger, string prefix)
        {
            return Policy.Handle<System.Exception>().WaitAndRetryAsync(
                retries,
                retry => TimeSpan.FromSeconds(15),
                (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(
                        exception,
                        "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                        prefix,
                        exception.GetType().Name,
                        exception.Message,
                        retry,
                        retries);
                }
            );
        }
    }

    private static IServiceCollection RegisterService<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        ServiceDescriptor serviceDescriptor = lifeTime switch
        {
            ServiceLifetime.Singleton => ServiceDescriptor.Singleton<TService, TImplementation>(),
            ServiceLifetime.Scoped => ServiceDescriptor.Scoped<TService, TImplementation>(),
            ServiceLifetime.Transient => ServiceDescriptor.Transient<TService, TImplementation>(),
            _ => throw new ArgumentOutOfRangeException(nameof(lifeTime), lifeTime, null)
        };
        services.Add(serviceDescriptor);
        return services;
    }
}
