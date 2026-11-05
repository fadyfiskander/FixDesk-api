namespace FixDesk.Application.RepairTickets.Dtos
{
    // Add-only notes with author and timestamp.
    public record TicketNoteDto(
        Guid Id,
        Guid RepairTicketId,
        string Content,
        Guid CreatedByUserId,
        DateTime CreatedAtUtc);
}
