using FixDesk.Domain.TicketQuotes;

namespace FixDesk.Application.RepairTickets.Dtos
{
    // One ticket quote version (section 10 of the reference).
    public record TicketQuoteDto(
        Guid Id,
        Guid RepairTicketId,
        int VersionNumber,
        long AmountInCents,
        string Reason,
        QuoteStatus Status,
        DateTime CreatedAtUtc,
        DateTime? ActionAtUtc,
        Guid? ActionByUserId,
        string? RejectReason);
}
