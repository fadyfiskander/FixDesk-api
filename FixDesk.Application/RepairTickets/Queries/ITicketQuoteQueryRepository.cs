using FixDesk.Application.RepairTickets.Dtos;

namespace FixDesk.Application.RepairTickets.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    // Children of the RepairTicket aggregate are read-only here; every write goes through the aggregate.
    public interface ITicketQuoteQueryRepository
    {
        Task<TicketQuoteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // All quote versions of one ticket, oldest first (section 10 of the reference).
        Task<IReadOnlyList<TicketQuoteDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default);
    }
}
