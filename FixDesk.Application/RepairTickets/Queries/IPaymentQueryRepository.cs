using FixDesk.Application.RepairTickets.Dtos;

namespace FixDesk.Application.RepairTickets.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    // Payments are immutable (section 5); corrections arrive as Reversal rows referencing OriginalPaymentId.
    public interface IPaymentQueryRepository
    {
        Task<PaymentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Payments of one ticket, chronological.
        Task<IReadOnlyList<PaymentDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default);
    }
}
