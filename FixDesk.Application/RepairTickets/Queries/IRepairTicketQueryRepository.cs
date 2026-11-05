using FixDesk.Application.RepairTickets.Dtos;

namespace FixDesk.Application.RepairTickets.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    // The detail DTO is composed with its children; the summary DTO carries the computed money.
    public interface IRepairTicketQueryRepository
    {
        // Full ticket card: root + children (quotes, payments, notes, history) + totals.
        Task<RepairTicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // By the readable ticket number (FD-000123) — the main screens search.
        Task<RepairTicketDto?> GetByTicketNumberAsync(long ticketNumber, CancellationToken cancellationToken = default);

        // Tickets list (WIP / history) with computed money.
        Task<IReadOnlyList<RepairTicketSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Customer history: all tickets of one customer.
        Task<IReadOnlyList<RepairTicketSummaryDto>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}
