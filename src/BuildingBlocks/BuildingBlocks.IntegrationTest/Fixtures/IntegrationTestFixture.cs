using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain;
using BuildingBlocks.Test.Factories;
using BuildingBlocks.Test.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace BuildingBlocks.Test.Fixtures
{
    public class IntegrationTestFixture<TEntryPoint, TDbContext> : IntegrationTestFixture<TEntryPoint>
        where TEntryPoint : class
        where TDbContext : DbContext
    {
        private readonly Checkpoint _checkpoint;

        public IntegrationTestFixture()
        {
            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
        }

        private async Task ResetState()
        {
            try
            {
                var connection = OptionsHelper.GetConnectionString("OrdersConnection");
                await _checkpoint.Reset(connection);
            }
            catch
            {
                // ignored
            }
        }

        public Task ExecuteDbContextAsync(Func<TDbContext, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()));

        public Task ExecuteDbContextAsync(Func<TDbContext, ValueTask> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()).AsTask());

        public Task ExecuteDbContextAsync(Func<TDbContext, IMediator, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>(), sp.GetService<IMediator>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, ValueTask<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>()).AsTask());

        public Task<T> ExecuteDbContextAsync<T>(Func<TDbContext, IMediator, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<TDbContext>(), sp.GetService<IMediator>()));

        public Task InsertAsync<T>(params T[] entities) where T : class
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Add(entity);
                }

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
            where TEntity : class
            where TEntity2 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);

                return db.SaveChangesAsync();
            });
        }

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2,
            TEntity3 entity3, TEntity4 entity4)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
            where TEntity4 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);
                db.Set<TEntity4>().Add(entity4);

                return db.SaveChangesAsync();
            });
        }

        public Task<T> FindAsync<T>(long id) where T : class
        {
            return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
        }

        public Task<T> FindFirstAsync<T>() where T : class
        {
            return ExecuteDbContextAsync(db => db.Set<T>().FirstOrDefaultAsync());
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await ResetState();
        }
    }

    public class IntegrationTestFixture<TEntryPoint> : IAsyncLifetime
        where TEntryPoint : class
    {
        protected readonly CustomApplicationFactory<TEntryPoint> Factory;
        // protected readonly LaunchSettingsFixture LaunchSettings;

        protected HttpClient Client => Factory.CreateClient();


        public IHttpClientFactory HttpClientFactory =>
            ServiceProvider.GetRequiredService<IHttpClientFactory>();

        public IHttpContextAccessor HttpContextAccessor =>
            ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        public IServiceProvider ServiceProvider => Factory.Services;
        public IConfiguration Configuration => Factory.Configuration;

        public IntegrationTestFixture()
        {
            Factory = new CustomApplicationFactory<TEntryPoint>();
        }

        public void SetOutput(ITestOutputHelper output)
        {
            Factory.OutputHelper = output;
            Factory.Server.AllowSynchronousIO = true;
        }

        public void RegisterTestServices(Action<IServiceCollection> services)
        {
            Factory.TestRegistrationServices = services;
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = ServiceProvider.CreateScope();

            await action(scope.ServiceProvider);
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = ServiceProvider.CreateScope();

            var result = await action(scope.ServiceProvider);

            return result;
        }

        public Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : class, INotification
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Publish(@event, cancellationToken);
            });
        }

        public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> request, CancellationToken cancellationToken)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request, cancellationToken);
            });
        }

        public Task SendAsync<T>(T request, CancellationToken cancellationToken) where T : class, ICommand
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(request, cancellationToken);
            });
        }

        public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
            where TResponse : class
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();

                return mediator.Send(query, cancellationToken);
            });
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            Factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}