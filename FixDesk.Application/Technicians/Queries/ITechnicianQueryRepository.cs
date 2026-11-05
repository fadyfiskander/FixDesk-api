using FixDesk.Application.Technicians.Dtos;

namespace FixDesk.Application.Technicians.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface ITechnicianQueryRepository
    {
        Task<TechnicianDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Picker of active technicians on the ticket screen.
        Task<IReadOnlyList<TechnicianDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        // The technician linked to a system account (may be absent).
        Task<TechnicianDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
