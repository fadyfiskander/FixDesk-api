using FixDesk.Domain.Abstractions;
using FixDesk.Domain.RepairTickets;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.TicketQuotes
{
    // نسخ سعر التذكرة (المرجع - قسم 10): نسخة واحدة معتمدة في كل وقت،
    // وموافقة العميل مرتبطة بنسخة مش flag. إدارته دايمًا من خلال RepairTicket.
    public class TicketQuote : Entity<TicketQuoteId>
    {
        public RepairTicketId RepairTicketId { get; private set; }

        // رقم النسخة (1, 2, 3...)
        public int VersionNumber { get; private set; }

        // سعر الإصلاح بالقروش (رسوم الكشف جزء منه لو وافق العميل)
        public long AmountInCents { get; private set; }

        // سبب النسخة (العطل الأولي أو العطل الإضافي)
        public string Reason { get; private set; } = null!;

        public QuoteStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        // وقت الموافقة أو الرفض ومين سجّلها
        public DateTime? ActionAtUtc { get; private set; }
        public Guid? ActionByUserId { get; private set; }

        // سبب الرفض (يُملأ فقط لو العميل رفض)
        public string? RejectReason { get; private set; }

        // للاستخدام في الـ EF فقط
        private TicketQuote()
        {
        }

        [SetsRequiredMembers]
        internal TicketQuote(
            RepairTicketId repairTicketId,
            int versionNumber,
            long amountInCents,
            string reason,
            DateTime createdAtUtc)
        {
            if (amountInCents <= 0)
                throw new ArgumentException("Quote amount must be greater than zero.", nameof(amountInCents));
            if (versionNumber <= 0)
                throw new ArgumentException("Quote version number must be greater than zero.", nameof(versionNumber));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Quote reason cannot be null or empty.", nameof(reason));

            Id = TicketQuoteId.New();
            RepairTicketId = repairTicketId;
            VersionNumber = versionNumber;
            AmountInCents = amountInCents;
            Reason = reason.Trim();
            Status = QuoteStatus.Pending;
            CreatedAtUtc = createdAtUtc;
        }

        internal void Approve(Guid byUserId, DateTime atUtc)
        {
            if (Status != QuoteStatus.Pending)
                throw new InvalidOperationException("Only a pending quote can be approved.");

            Status = QuoteStatus.Approved;
            ActionAtUtc = atUtc;
            ActionByUserId = byUserId;
        }

        internal void Reject(Guid byUserId, string? rejectReason, DateTime atUtc)
        {
            if (Status != QuoteStatus.Pending)
                throw new InvalidOperationException("Only a pending quote can be rejected.");

            Status = QuoteStatus.Rejected;
            RejectReason = rejectReason?.Trim();
            ActionAtUtc = atUtc;
            ActionByUserId = byUserId;
        }

        // عند إضافة نسخة جديدة: النسخ القديمة (المعلقة والمعتمدة) تتحوّل لـ Superseded
        internal void Supersede(Guid byUserId, DateTime atUtc)
        {
            if (Status is not (QuoteStatus.Pending or QuoteStatus.Approved))
                return;

            Status = QuoteStatus.Superseded;
            ActionAtUtc = atUtc;
            ActionByUserId = byUserId;
        }
    }
}
