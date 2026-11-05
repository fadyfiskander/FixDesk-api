using FixDesk.Domain.Brands;
using FixDesk.Domain.Brands.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class BrandRepository : EfRepository<Brand, BrandId>, IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context)
        {
        }

        public override Task DeleteAsync(Brand entity, CancellationToken cancellationToken = default)
        {
            entity.Deactivate();
            _context.Set<Brand>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
