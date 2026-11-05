using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.RepairTickets.Repositories
{
    // الملاحظات children بتاعة الـ RepairTicket aggregate — Read فقط (Dapper):
    // الإضافة من خلال RepairTicket.AddNote، ومفيش تعديل ولا حذف (سجل).
    public interface ITicketNoteQueryRepository : IQueryRepository<TicketNote, Guid>
    {
        // كل الملاحظات لتذكرة معينة
        Task<IReadOnlyList<TicketNote>> GetAllByTicketIdAsync(RepairTicketId repairTicketId, CancellationToken cancellationToken = default);
    }
}
