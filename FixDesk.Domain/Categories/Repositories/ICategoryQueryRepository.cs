using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Categories.Repositories
{
    // الـ Read (Dapper) للـ Category.
    public interface ICategoryQueryRepository : IQueryRepository<Category, CategoryId>
    {
        // لائحة الـ Lookup الفعالة بس (شاشة فتح التذكرة)
        Task<IReadOnlyList<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    }
}
