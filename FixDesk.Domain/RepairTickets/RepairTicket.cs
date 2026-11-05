using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Customers;
using FixDesk.Domain.Devices;
using FixDesk.Domain.Payments;
using FixDesk.Domain.TicketQuotes;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.RepairTickets
{
    // محور النظام (المرجع - قسم 6). تشيل CustomerId بنفسها، وفيها الجهاز والعطل، الملحقات،
    // حالة الاستلام، رسوم الكشف، النسخ، الضمان، الدفعات والملاحظات وسجل الحالات.
    public class RepairTicket : AggregateRoot<RepairTicketId>
    {
        // رقم مقروء مثل FD-000123 — الـ Sequence في الـ Application layer.
        public long TicketNumber { get; private set; }
        public string TicketNumberDisplay => $"FD-{TicketNumber:D6}";

        public CustomerId CustomerId { get; private set; }
        public DeviceId DeviceId { get; private set; }

        // العطل المذكور من العميل وقت الاستلام
        public string ReportedIssue { get; private set; }

        // الملحقات وقت الاستلام
        private readonly List<DeviceAttachment> _accessories = [];
        public IReadOnlyCollection<DeviceAttachment> Accessories => _accessories.AsReadOnly();

        // حالة الجهاز وقت الاستلام
        public DeviceCondition ReceptionCondition { get; private set; }

        public RepairTicketState TicketState { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? ExpectedDeliveryAtUtc { get; private set; }

        // رسوم الكشف — تنجمع من الإعدادات وقت فتح التذكرة (المرجع ~ قسم 9)،
        // وOwner/Staff يقدروا يعدلوها أو يصفروها في تذكرة معينة للحظ.
        public long? InspectionFeeInCents { get; private set; }

        public string? Diagnosis { get; private set; }

        // الفني (اختياري)
        public Guid? AssignedTechnicianId { get; private set; }

        // بيانات الضمان — بتُتسجل عند التسليم (المرجع - قسم 11).
        public int? WarrantyDays { get; private set; }
        public DateTime? WarrantyStartDateUtc { get; private set; }

        // ===== Children (متقسم في الـ Infrastructure) =====
        private readonly List<TicketQuote> _quotes = [];
        public IReadOnlyCollection<TicketQuote> Quotes => _quotes.AsReadOnly();

        private readonly List<TicketStatusHistory> _statusHistory = [];
        public IReadOnlyCollection<TicketStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

        private readonly List<TicketNote> _notes = [];
        public IReadOnlyCollection<TicketNote> Notes => _notes.AsReadOnly();

        private readonly List<Payment> _payments = [];
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        // ===== الحالات ذات المسار واحد ورا واحد (المرجع - قسم 7) =====
        private static readonly Dictionary<RepairTicketState, RepairTicketState[]> AllowedTransitions = new()
        {
            [RepairTicketState.Received]          = [RepairTicketState.AwaitingApproval, RepairTicketState.Cancelled],
            [RepairTicketState.AwaitingApproval]  = [RepairTicketState.InRepair, RepairTicketState.Rejected],
            [RepairTicketState.InRepair]          = [RepairTicketState.Ready, RepairTicketState.AwaitingApproval],
            [RepairTicketState.Ready]             = [RepairTicketState.Delivered],
            [RepairTicketState.Rejected]          = [RepairTicketState.Delivered, RepairTicketState.Cancelled],
            [RepairTicketState.Delivered]         = [],
            [RepairTicketState.Cancelled]         = [],
        };

        [SetsRequiredMembers]
        private RepairTicket(
            long ticketNumber,
            CustomerId customerId,
            DeviceId deviceId,
            string reportedIssue,
            DeviceCondition receptionCondition,
            long? inspectionFeeInCents,
            DateTime? expectedDeliveryAtUtc,
            Guid createdByUserId,
            DateTime createdAtUtc,
            IEnumerable<DeviceAttachment>? accessories)
        {
            if (ticketNumber <= 0)
                throw new ArgumentException("Ticket number must be greater than zero.", nameof(ticketNumber));
            if (string.IsNullOrWhiteSpace(reportedIssue))
                throw new ArgumentException("Reported issue cannot be null or empty.", nameof(reportedIssue));

            Id = RepairTicketId.New();
            TicketNumber = ticketNumber;
            CustomerId = customerId;
            DeviceId = deviceId;
            ReportedIssue = reportedIssue.Trim();
            ReceptionCondition = receptionCondition;
            InspectionFeeInCents = inspectionFeeInCents > 0 ? inspectionFeeInCents : null;
            CreatedAtUtc = createdAtUtc;
            ExpectedDeliveryAtUtc = expectedDeliveryAtUtc;
            AssignedTechnicianId = null;
            TicketState = RepairTicketState.Received;

            if (accessories is not null)
            {
                foreach (var accessory in accessories)
                {
                    if (accessory is null)
                        continue;
                    ValidateAccessory(accessory);
                    _accessories.Add(accessory);
                }
            }

            // أول تدخّل في الـ audit: التذكرة اتفتح (من null إلى Received).
            RecordStateChange(fromState: null, RepairTicketState.Received, createdByUserId, createdAtUtc);
        }

        // للاستخدام في الـ EF فقط (الماده materialization من الداتابيز)
#pragma warning disable CS8618 // non-nullable fields will be set by EF
        [SetsRequiredMembers]
        private RepairTicket()
        {
        }
#pragma warning restore CS8618

        public static RepairTicket Create(
            long ticketNumber,
            CustomerId customerId,
            DeviceId deviceId,
            string reportedIssue,
            DeviceCondition receptionCondition,
            Guid createdByUserId,
            DateTime createdAtUtc,
            long? inspectionFeeInCents = null,
            DateTime? expectedDeliveryAtUtc = null,
            IEnumerable<DeviceAttachment>? accessories = null)
        {
            return new RepairTicket(
                ticketNumber, customerId, deviceId, reportedIssue, receptionCondition,
                inspectionFeeInCents, expectedDeliveryAtUtc, createdByUserId, createdAtUtc, accessories);
        }

        // ===== تحديث حقول التعريق =====
        public void UpdateReportedIssue(string reportedIssue)
        {
            EnsureNotClosed();
            if (string.IsNullOrWhiteSpace(reportedIssue))
                throw new ArgumentException("Reported issue cannot be null or empty.", nameof(reportedIssue));
            ReportedIssue = reportedIssue.Trim();
        }

        public void UpdateDiagnosis(string diagnosis)
        {
            EnsureNotClosed();
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnosis cannot be null or empty.", nameof(diagnosis));
            Diagnosis = diagnosis.Trim();
        }

        public void SetExpectedDelivery(DateTime? expectedDeliveryAtUtc)
        {
            EnsureNotClosed();
            ExpectedDeliveryAtUtc = expectedDeliveryAtUtc;
        }

        public void SetInspectionFee(long? inspectionFeeInCents)
        {
            EnsureNotClosed();
            InspectionFeeInCents = inspectionFeeInCents > 0 ? inspectionFeeInCents : null;
        }

        public void AssignTechnician(Guid? technicianUserId)
        {
            EnsureNotClosed();
            AssignedTechnicianId = technicianUserId;
        }

        public void UpdateReceptionCondition(string physicalStatus, string defectDescription)
        {
            EnsureNotClosed();
            var updated = DeviceCondition.Create(physicalStatus, defectDescription);
            if (ReceptionCondition.Equals(updated))
                return;
            ReceptionCondition = updated;
        }

        public void AddAccessory(DeviceAttachment accessory)
        {
            EnsureNotClosed();
            if (accessory is null)
                throw new ArgumentNullException(nameof(accessory), "Accessory cannot be null.");
            ValidateAccessory(accessory);
            _accessories.Add(accessory);
        }

        public void RemoveAccessory(string accessoryName)
        {
            EnsureNotClosed();
            if (string.IsNullOrWhiteSpace(accessoryName))
                throw new ArgumentException("Accessory name cannot be null or empty.", nameof(accessoryName));

            var found = _accessories.FirstOrDefault(a =>
                string.Equals(a.Name, accessoryName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (found is null)
                throw new InvalidOperationException($"Accessory '{accessoryName}' was not found on this ticket.");

            _accessories.Remove(found);
        }

        // ===== الحالات (state machine بسيطة: مكان واحد بيتسجل فيه كل حرف) =====
        public void TransitionTo(
            RepairTicketState to,
            Guid actingUserId,
            DateTime atUtc,
            int? warrantyDays = null,
            bool allowOutstandingBalance = false)
        {
            if (!AllowedTransitions[TicketState].Contains(to))
                throw new InvalidOperationException(
                    $"Transition from '{TicketState}' to '{to}' is not allowed.");

            // المرجع ~ القاعدة 1: مفيش إصلاح قبل موافقة العميل على السعر.
            if (to == RepairTicketState.InRepair && GetApprovedQuote() is null)
                throw new InvalidOperationException(
                    "Repair cannot start before the customer approves the price.");

            // المرجع ~ القاعدة 2: مفيش تسليم يبيع متبقي، بدون استثناء المالك.
            if (to == RepairTicketState.Delivered && GetRemainingAmountInCents() > 0 && !allowOutstandingBalance)
                throw new InvalidOperationException(
                    "The ticket cannot be delivered before the remaining amount is fully paid.");

            if (to == RepairTicketState.Delivered)
            {
                // المرجع ~ قسم 11: الضمان يت سجل عند التسليم.
                if (warrantyDays is { } days && days < 0)
                    throw new ArgumentException("Warranty days cannot be negative.", nameof(warrantyDays));
                WarrantyDays = warrantyDays ?? 0;
                WarrantyStartDateUtc = atUtc;
            }

            var fromState = TicketState;
            TicketState = to;
            RecordStateChange(fromState, to, actingUserId, atUtc);
        }

        // ===== الت outubro (المرجع - قسم 10) =====
        public TicketQuote SubmitQuote(long amountInCents, string reason, Guid byUserId, DateTime atUtc)
        {
            EnsureNotClosed();

            int nextVersion = _quotes.Count == 0 ? 1 : _quotes.Max(q => q.VersionNumber) + 1;
            var quote = new TicketQuote(Id, nextVersion, amountInCents, reason, atUtc);
            _quotes.Add(quote);

            // نسخة واحدة معتمدة في كل وقت: النسخ القديمة (المعلقة والمعتمدة) تتحوّل لـ Superseded.
            foreach (var oldQuote in _quotes.Where(q => ReferenceEquals(q, quote) == false))
                oldQuote.Supersede(byUserId, atUtc);

            return quote;
        }

        public void ApproveQuote(TicketQuoteId quoteId, Guid byUserId, DateTime atUtc)
        {
            EnsureNotClosed();
            var quote = FindQuote(quoteId);
            quote.Approve(byUserId, atUtc);
        }

        public void RejectQuote(TicketQuoteId quoteId, string? rejectReason, Guid byUserId, DateTime atUtc)
        {
            EnsureNotClosed();
            var quote = FindQuote(quoteId);
            quote.Reject(byUserId, rejectReason, atUtc);
        }

        // ===== الدفعات (المرجع ~ القاعدة 5: لا تعديل ولا حذف — التصحيح بقيد عكسي) =====
        public Payment RecordPayment(
            long amountInCents,
            Guid byUserId,
            DateTime atUtc,
            PaymentType type = PaymentType.Payment,
            Guid? originalPaymentId = null)
        {
            EnsureNotClosed();
            if (amountInCents <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amountInCents));

            if (type == PaymentType.Reversal)
            {
                if (originalPaymentId is null)
                    throw new ArgumentException(
                        "A reversal payment must reference the original payment it corrects.",
                        nameof(originalPaymentId));
                if (_payments.All(p => p.Id != originalPaymentId.Value))
                    throw new InvalidOperationException("The referenced original payment does not exist on this ticket.");
            }
            else if (originalPaymentId is not null)
            {
                throw new ArgumentException("Only a reversal payment references an original payment.", nameof(originalPaymentId));
            }

            var payment = new Payment(Id, type, amountInCents, originalPaymentId, byUserId, atUtc);
            _payments.Add(payment);
            return payment;
        }

        // ===== الملاحظات (ملاحظات متعددة بتاريخ وكاتبها؛ لا حذف — سجل) =====
        public void AddNote(string content, Guid byUserId, DateTime atUtc)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Note content cannot be null or empty.", nameof(content));

            _notes.Add(new TicketNote(Id, content, byUserId, atUtc));
        }

        // ===== الحسابات (كمبو **لا ما يتخزنها المقرر ~ قسم 9**، السالب محسوب) =====
        public TicketQuote? GetApprovedQuote() =>
            _quotes.FirstOrDefault(q => q.Status == QuoteStatus.Approved);

        public TicketQuote? GetLatestQuote() =>
            _quotes.OrderByDescending(q => q.VersionNumber).FirstOrDefault();

        public long GetDueAmountInCents()
        {
            if (GetApprovedQuote() is { } approved)
                return approved.AmountInCents;

            var inspectionFee = InspectionFeeInCents ?? 0;
            var latestAmount = GetLatestQuote()?.AmountInCents;

            // تم رفض السعر: المستحق = رسوم الكشف، بس مش أكتر من سعر الإصلاح نفسه.
            if (TicketState == RepairTicketState.Rejected && latestAmount is { } amount)
                return Math.Min(inspectionFee, amount);

            return inspectionFee;
        }

        public long GetTotalPaidInCents()
        {
            var paid = _payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.AmountInCents);
            var reversed = _payments.Where(p => p.Type == PaymentType.Reversal).Sum(p => p.AmountInCents);
            return paid - reversed;
        }

        public long GetRemainingAmountInCents() =>
            GetDueAmountInCents() - GetTotalPaidInCents();

        // تنبيه "داخل الضمان" — الحكم إنته العطل نفسه قرار إنساني من الـ Application layer.
        public bool IsUnderWarranty(DateTime atUtc)
        {
            if (WarrantyStartDateUtc is not { } start)
                return false;
            if (WarrantyDays is not { } days || days <= 0)
                return false;
            return atUtc <= start.AddDays(days);
        }

        public bool IsClosed() =>
            TicketState is RepairTicketState.Delivered or RepairTicketState.Cancelled;

        // ===== private =====
        private void RecordStateChange(RepairTicketState? fromState, RepairTicketState toState, Guid byUserId, DateTime atUtc)
        {
            _statusHistory.Add(new TicketStatusHistory(Id, fromState, toState, byUserId, atUtc));
        }

        private TicketQuote FindQuote(TicketQuoteId quoteId)
        {
            var quote = _quotes.FirstOrDefault(q => q.Id == quoteId)
                ?? throw new InvalidOperationException("The specified quote does not exist on this ticket.");
            return quote;
        }

        private void EnsureNotClosed()
        {
            if (IsClosed())
                throw new InvalidOperationException("A closed (delivered or cancelled) ticket cannot be modified.");
        }

        private static void ValidateAccessory(DeviceAttachment accessory)
        {
            if (string.IsNullOrWhiteSpace(accessory.Name))
                throw new ArgumentException("Accessory name cannot be null or empty.", nameof(accessory));
        }
    }
}
