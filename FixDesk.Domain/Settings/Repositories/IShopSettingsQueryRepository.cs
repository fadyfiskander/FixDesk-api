using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Settings.Repositories
{
    // الـ Read (Dapper) للإعدادات.
    public interface IShopSettingsQueryRepository : IQueryRepository<ShopSettings, Guid>
    {
        // الإعدادات الوحيدة (SingleRow) — ترجع null لو أول تشغيل أو ما اتسجلتش
        Task<ShopSettings?> GetAsync(CancellationToken cancellationToken = default);
    }
}
