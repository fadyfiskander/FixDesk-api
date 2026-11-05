using FixDesk.Application.Settings.Dtos;

namespace FixDesk.Application.Settings.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface IShopSettingsQueryRepository
    {
        // Single-row settings; null until the owner saves for the first time.
        Task<ShopSettingsDto?> GetAsync(CancellationToken cancellationToken = default);
    }
}
