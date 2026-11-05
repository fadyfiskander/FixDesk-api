using FixDesk.Domain.Abstractions;
using FixDesk.Domain.RepairTickets;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Payments
{
    // الدفعة: مبلغ دائماً موجب بالقروش. التصحيح بقيد عكسي (Reversal) مش تعديل ولا حذف.
    // الـ Owner هو اللي يقدر يعمل Reversal — يقيد الـ policies في الـ Application layer.
    public class Payment : Entity<Guid>
    {
        public RepairTicketId RepairTicketId { get; private set; }

        // Payment = دفععة، Reversal = قيد عكسي (رد دفعت أو رد فلوس لعميل)
        public PaymentType Type { get; private set; }

        public long AmountInCents { get; private set; }

        // للـ Reversal: الدفعت الأصلية اللي بيتم تصحيحها
        public Guid? OriginalPaymentId { get; private set; }

        public Guid CreatedByUserId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private Payment()
        {
        }

        [SetsRequiredMembers]
        internal Payment(
            RepairTicketId repairTicketId,
            PaymentType type,
            long amountInCents,
            Guid? originalPaymentId,
            Guid createdByUserId,
            DateTime createdAtUtc)
        {
            if (amountInCents <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amountInCents));
            if (type == PaymentType.Reversal && originalPaymentId is null)
                throw new ArgumentException("A reversal payment must reference an original payment.", nameof(originalPaymentId));

            Id = Guid.NewGuid();
            RepairTicketId = repairTicketId;
            Type = type;
            AmountInCents = amountInCents;
            OriginalPaymentId = originalPaymentId;
            CreatedByUserId = createdByUserId;
            CreatedAtUtc = createdAtUtc;
        }
    }
}
