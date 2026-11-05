using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Brands.Repositories
{
    // الـ Write (EF Core): إضافة/تعديل.
    // ملاحظة: مفيش Soft Delete للـ Lookup — الحذف المنطقي هو Deactivate (IsActive = false).
    public interface IBrandRepository : IRepository<Brand, BrandId>
    {
    }
}
