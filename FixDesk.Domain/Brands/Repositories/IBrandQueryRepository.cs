using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Brands.Repositories
{
    // الـ Read (Dapper) للـ Brand.
    public interface IBrandQueryRepository : IQueryRepository<Brand, BrandId>
    {
        // لائحة الـ Lookup الفعالة بس (شاشة فتح التذكرة)
        Task<IReadOnlyList<Brand>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    }
}
