using FixDesk.Domain.Abstractions.Repositories;
using FixDesk.Domain.Customers;

namespace FixDesk.Domain.RepairTickets.Repositories
{
    // الـ Read (Dapper) للتذكرة.
    public interface IRepairTicketQueryRepository : IQueryRepository<RepairTicket, RepairTicketId>
    {
        // بالرقم المقروء (FD-000123) — البحث الرئيسي في الشاشات
        Task<RepairTicket?> GetByTicketNumberAsync(long ticketNumber, CancellationToken cancellationToken = default);

        // تاريخ العميل (كل تذاكره) — شاشة تاريخ تاريخ العميل والجهاز
        Task<IReadOnlyList<RepairTicket>> GetAllByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    }
}
