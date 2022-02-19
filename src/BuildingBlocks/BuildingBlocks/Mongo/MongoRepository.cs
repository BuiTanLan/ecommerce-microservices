using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Specification;
using BuildingBlocks.Core.Domain.Model;
using MongoDB.Driver;

namespace BuildingBlocks.Mongo
{
    public class MongoRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : IAggregateRoot<TId>
    {
        private readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TEntity> DbSet;

        public MongoRepository(IMongoDbContext context)
        {
            _context = context;
            DbSet = _context.GetCollection<TEntity>();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public Task<TEntity?> FindByIdAsync(IIdentity<TId> id, CancellationToken cancellationToken = default)
        {
            return FindOneAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return DbSet.Find(predicate).SingleOrDefaultAsync(cancellationToken: cancellationToken)!;
        }

        public Task<TEntity?> FindOneAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TEntity>> FindAsync(
            ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Find(predicate).ToListAsync(cancellationToken: cancellationToken)!;
        }

        public async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.AsQueryable().ToListAsync(cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity, new ReplaceOptions(), cancellationToken);

            return entity;
        }

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return DbSet.DeleteOneAsync(e => entities.Any(i => e.Id.Equals(i.Id)), cancellationToken);
        }

        public Task DeleteAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => DbSet.DeleteOneAsync(predicate, cancellationToken);

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return DbSet.DeleteOneAsync(e => e.Id.Equals(entity.Id), cancellationToken);
        }
    }
}
