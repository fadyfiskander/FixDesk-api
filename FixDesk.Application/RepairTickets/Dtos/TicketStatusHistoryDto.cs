using FixDesk.Domain.RepairTickets;

namespace FixDesk.Application.RepairTickets.Dtos
{
    // Audit trail of the ticket state machine (FromState = null means ticket creation).
    public record TicketStatusHistoryDto(
        Guid Id,
        Guid RepairTicketId,
        RepairTicketState? FromState,
        RepairTicketState ToState,
        Guid ChangedByUserId,
        DateTime ChangedAtUtc);
}
