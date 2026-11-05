using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.RepairTickets.Repositories
{
    // الـ Write (EF Core): إضافة/تعديل كـ Aggregate (التذكرة بتتشيل children معاها).
    // ملاحظة: مفيش حذف للتذاكر — الإلغاء بالحالة Cancelled.
    // DeleteAsync (للاتساق مع العقد العام) هيرمي NotSupportedException في الـ Infrastructure.
    public interface IRepairTicketRepository : IRepository<RepairTicket, RepairTicketId>
    {
    }
}
