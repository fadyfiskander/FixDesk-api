using FixDesk.Application.Brands.Dtos;

namespace FixDesk.Application.Brands.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface IBrandQueryRepository
    {
        Task<BrandDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BrandDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Lookup list for pickers (active only).
        Task<IReadOnlyList<BrandDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    }
}
