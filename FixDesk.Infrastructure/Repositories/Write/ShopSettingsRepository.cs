using FixDesk.Domain.Settings;
using FixDesk.Domain.Settings.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class ShopSettingsRepository : EfRepository<ShopSettings, Guid>, IShopSettingsRepository
    {
        public ShopSettingsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
