using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.RepairTickets
{
    // سجل تغيير الحالات (المرجع ~ القاعدة 4): مين، إمتى، من إيه لإيه. Add-only.
    public class TicketStatusHistory : Entity<Guid>
    {
        public RepairTicketId RepairTicketId { get; private set; }

        // null = فتح التذكرة ذاتها (الانتقال الأول)
        public RepairTicketState? FromState { get; private set; }

        public RepairTicketState ToState { get; private set; }
        public Guid ChangedByUserId { get; private set; }
        public DateTime ChangedAtUtc { get; private set; }

        private TicketStatusHistory()
        {
        }

        [SetsRequiredMembers]
        internal TicketStatusHistory(
            RepairTicketId repairTicketId,
            RepairTicketState? fromState,
            RepairTicketState toState,
            Guid changedByUserId,
            DateTime changedAtUtc)
        {
            Id = Guid.NewGuid();
            RepairTicketId = repairTicketId;
            FromState = fromState;
            ToState = toState;
            ChangedByUserId = changedByUserId;
            ChangedAtUtc = changedAtUtc;
        }
    }
}
