using FixDesk.Domain.Abstractions.Repositories;
using FixDesk.Domain.RepairTickets;

namespace FixDesk.Domain.Payments.Repositories
{
    // الدفعات children بتاعة الـ RepairTicket aggregate — Read فقط (Dapper):
    // تسجيل الدفعات (والقيد العكسي) بيتم من خلال RepairTicket.RecordPayment
    // (المرجع ~ القاعدة 5: الدفعات لا تتعدل ولا تتحذف، والقاعدة 8: دفعة + حالة في transaction واحدة).
    public interface IPaymentQueryRepository : IQueryRepository<Payment, Guid>
    {
        // كل الدفعات (والقيد العكسي) لتذكرة معينة
        Task<IReadOnlyList<Payment>> GetAllByTicketIdAsync(RepairTicketId repairTicketId, CancellationToken cancellationToken = default);
    }
}
