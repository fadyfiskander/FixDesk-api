using FixDesk.Domain.Payments;

namespace FixDesk.Application.RepairTickets.Dtos
{
    // Payments are immutable (section 5). Corrections happen via Reversal referencing OriginalPaymentId.
    public record PaymentDto(
        Guid Id,
        Guid RepairTicketId,
        PaymentType Type,
        long AmountInCents,
        Guid? OriginalPaymentId,
        Guid CreatedByUserId,
        DateTime CreatedAtUtc);
}
