using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Abstractions.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public abstract class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        protected readonly AppDbContext _context;

        protected EfRepository(AppDbContext context)
        {
            _context = context;
        }

        public async virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

        public async virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            _context.Set<TEntity>().Update(entity);

        public async virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException($"Delete is not supported for {typeof(TEntity).Name}.");
    }
}