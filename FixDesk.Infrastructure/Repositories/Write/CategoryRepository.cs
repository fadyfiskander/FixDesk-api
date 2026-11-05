using FixDesk.Domain.Categories;
using FixDesk.Domain.Categories.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class CategoryRepository : EfRepository<Category, CategoryId>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public override Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
        {
            entity.Deactivate();
            _context.Set<Category>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
