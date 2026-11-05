using FixDesk.Domain.RepairTickets;

namespace FixDesk.Application.RepairTickets.Dtos
{
    // Row for the tickets list screens (WIP / history).
    // Money stays in cents (INTEGER financial rule); the UI formats it.
    public record RepairTicketSummaryDto(
        Guid Id,
        long TicketNumber,
        RepairTicketState TicketState,
        DateTime CreatedAtUtc,
        DateTime? ExpectedDeliveryAtUtc,
        Guid CustomerId,
        string CustomerName,
        string CustomerPhoneNumber,
        Guid DeviceId,
        string DeviceModel,
        long DueAmountInCents,
        long TotalPaidInCents,
        long RemainingAmountInCents)
    {
        public string TicketNumberDisplay => $"FD-{TicketNumber:D6}";
    }
}
