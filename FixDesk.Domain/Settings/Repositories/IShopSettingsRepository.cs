using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Settings.Repositories
{
    // الـ Write (EF Core): صف واحد فقط (Id = ShopSettings.SingleRowId).
    // AddAsync يُستخدم لإنشاء الصف أول مرة، UpdateAsync للتحديث، DeleteAsync مش له معنى هنا.
    public interface IShopSettingsRepository : IRepository<ShopSettings, Guid>
    {
    }
}
