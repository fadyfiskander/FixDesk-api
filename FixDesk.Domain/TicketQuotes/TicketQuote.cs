using FixDesk.Domain.Abstractions;
using FixDesk.Domain.RepairTickets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FixDesk.Domain.TicketQuotes
{
    public class TicketQuote : AggregateRoot<TicketQuoteId>
    {
        public RepairTicketId RepairTicketId { get; private set; }

        public int VersionNumber { get; private set; } // رقم النسخة (1, 2, 3...)
        public long AmountInCents { get; private set; } // المبلغ الكلي للإصلاح بالقروش
        public string Reason { get; private set; } // سبب الإصلاح أو سبب التعديل
        public QuoteStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? ActionAt { get; private set; } // وقت الموافقة أو الرفض
        public Guid? ActionByUserId { get; private set; } // مين اللي سجل الإجراء (Staff/Owner)
        public string RejectReason { get; private set; } // يُملأ فقط لو العميل رفض

        [SetsRequiredMembers]
        private TicketQuote(RepairTicketId repairTicketId, int versionNumber, long amountInCents, string reason)
        {
            if (amountInCents < 0) throw new ArgumentException("المبلغ لا يمكن أن يكون بالسالب");

            Id = TicketQuoteId.New();
            RepairTicketId = repairTicketId;
            VersionNumber = versionNumber;
            AmountInCents = amountInCents;
            Reason = reason;
            Status = QuoteStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public static TicketQuote Create(RepairTicketId repairTicketId, int versionNumber, long amountInCents, string reason)
        {
            return new TicketQuote(repairTicketId, versionNumber, amountInCents, reason);
        }

        public void Approve(Guid userId)
        {
            Status = QuoteStatus.Approved;
            ActionAt = DateTime.UtcNow;
            ActionByUserId = userId;
        }

        public void Reject(Guid userId, string rejectReason)
        {
            Status = QuoteStatus.Rejected;
            ActionAt = DateTime.UtcNow;
            ActionByUserId = userId;
            RejectReason = rejectReason;
        }

        public void Supersede()
        {
            Status = QuoteStatus.Superseded;
        }
    }
}
