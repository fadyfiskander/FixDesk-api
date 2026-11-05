using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.RepairTickets.Repositories
{
    // سجل الحالات children بتاعة الـ RepairTicket aggregate — Read فقط (Dapper):
    // التغيير بيتم تلقائيًا من RepairTicket.TransitionTo (Add-only).
    public interface ITicketStatusHistoryQueryRepository : IQueryRepository<TicketStatusHistory, Guid>
    {
        // كل سجل الحالات لتذكرة معينة (بالترتيب)
        Task<IReadOnlyList<TicketStatusHistory>> GetAllByTicketIdAsync(RepairTicketId repairTicketId, CancellationToken cancellationToken = default);
    }
}
