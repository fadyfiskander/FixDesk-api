using FixDesk.Domain.RepairTickets;

namespace FixDesk.Application.RepairTickets.Dtos
{
    // Full aggregate view: root + children + computed totals.
    // Computed money fields come from RepairTicket.GetDueAmountInCents / GetTotalPaidInCents / GetRemainingAmountInCents.
    public record RepairTicketDto(
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
        string? DeviceSerialNumber,
        string ReportedIssue,
        DeviceConditionDto ReceptionCondition,
        IReadOnlyList<DeviceAttachment> Accessories,
        long? InspectionFeeInCents,
        string? Diagnosis,
        Guid? AssignedTechnicianId,
        int? WarrantyDays,
        DateTime? WarrantyStartDateUtc,
        long DueAmountInCents,
        long TotalPaidInCents,
        long RemainingAmountInCents,
        IReadOnlyList<TicketQuoteDto> Quotes,
        IReadOnlyList<PaymentDto> Payments,
        IReadOnlyList<TicketNoteDto> Notes,
        IReadOnlyList<TicketStatusHistoryDto> StatusHistory)
    {
        public string TicketNumberDisplay => $"FD-{TicketNumber:D6}";
    }
}
