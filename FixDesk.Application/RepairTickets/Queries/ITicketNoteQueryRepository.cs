using FixDesk.Application.RepairTickets.Dtos;

namespace FixDesk.Application.RepairTickets.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface ITicketNoteQueryRepository
    {
        Task<TicketNoteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Notes of one ticket, chronological.
        Task<IReadOnlyList<TicketNoteDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default);
    }
}
