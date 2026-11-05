using FixDesk.Application.RepairTickets.Dtos;

namespace FixDesk.Application.RepairTickets.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface ITicketStatusHistoryQueryRepository
    {
        Task<TicketStatusHistoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // State-machine audit trail of one ticket, chronological.
        Task<IReadOnlyList<TicketStatusHistoryDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default);
    }
}
