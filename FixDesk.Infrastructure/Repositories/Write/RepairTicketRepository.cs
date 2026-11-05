using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Payments;
using FixDesk.Domain.RepairTickets;
using FixDesk.Domain.RepairTickets.Repositories;
using FixDesk.Domain.TicketQuotes;
using FixDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class RepairTicketRepository : EfRepository<RepairTicket, RepairTicketId>, IRepairTicketRepository
    {
        public RepairTicketRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task UpdateAsync(RepairTicket entity, CancellationToken cancellationToken = default)
        {
            // Root-only state assignment: EntityEntry.State does not recurse into children.
            // Children are tracked explicitly below (Added only when they do not exist yet),
            // so a payment added through the aggregate is INSERTed while existing rows are left alone.
            _context.Entry(entity).State = EntityState.Modified;

            await TrackNewChildrenAsync<TicketQuote, TicketQuoteId>(entity.Quotes, cancellationToken);
            await TrackNewChildrenAsync<Payment, Guid>(entity.Payments, cancellationToken);
            await TrackNewChildrenAsync<TicketNote, Guid>(entity.Notes, cancellationToken);
            await TrackNewChildrenAsync<TicketStatusHistory, Guid>(entity.StatusHistory, cancellationToken);
        }

        public override Task DeleteAsync(RepairTicket entity, CancellationToken cancellationToken = default)
            => throw new NotSupportedException(
                "Tickets are never deleted; use the Cancelled state instead.");

        private async Task TrackNewChildrenAsync<TChild, TChildKey>(
            IEnumerable<TChild> children,
            CancellationToken cancellationToken)
            where TChild : Entity<TChildKey>
        {
            // Snapshot: EF relationship fixup may mutate the aggregate's collection during Add.
            foreach (var child in children.ToArray())
            {
                if (_context.Entry(child).State != EntityState.Detached)
                    continue;

                var existsInDatabase = await _context.Set<TChild>()
                    .FindAsync([child.Id!], cancellationToken) is not null;

                if (!existsInDatabase)
                    _context.Add(child);
            }
        }
    }
}
