using FixDesk.Application.Categories.Dtos;

namespace FixDesk.Application.Categories.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface ICategoryQueryRepository
    {
        Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Lookup list for pickers (active only).
        Task<IReadOnlyList<CategoryDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    }
}
