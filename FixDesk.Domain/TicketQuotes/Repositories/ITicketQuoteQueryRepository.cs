using FixDesk.Domain.Abstractions.Repositories;
using FixDesk.Domain.RepairTickets;

namespace FixDesk.Domain.TicketQuotes.Repositories
{
    // نسخ السعر children بتاعة الـ RepairTicket aggregate — Read فقط (Dapper):
    // إنشاء/اعتماد/رفض النسخ فيه من خلال RepairTicket (نسخة معتمدة واحدة في كل وقت).
    public interface ITicketQuoteQueryRepository : IQueryRepository<TicketQuote, TicketQuoteId>
    {
        // كل النسخ لتذكرة معينة (بالترتيب)
        Task<IReadOnlyList<TicketQuote>> GetAllByTicketIdAsync(RepairTicketId repairTicketId, CancellationToken cancellationToken = default);
    }
}
