using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.RepairTickets
{
    // ملاحظات متعددة بتاريخ وكاتبها (المرجع - قسم 5). Add-only.
    public class TicketNote : Entity<Guid>
    {
        public RepairTicketId RepairTicketId { get; private set; }
        public string Content { get; private set; } = null!;
        public Guid CreatedByUserId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private TicketNote()
        {
        }

        [SetsRequiredMembers]
        internal TicketNote(RepairTicketId repairTicketId, string content, Guid createdByUserId, DateTime createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Note content cannot be null or empty.", nameof(content));

            Id = Guid.NewGuid();
            RepairTicketId = repairTicketId;
            Content = content.Trim();
            CreatedByUserId = createdByUserId;
            CreatedAtUtc = createdAtUtc;
        }
    }
}
