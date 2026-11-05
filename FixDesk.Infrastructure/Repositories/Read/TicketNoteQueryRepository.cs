using System.Data;
using Dapper;
using FixDesk.Application.RepairTickets.Dtos;
using FixDesk.Application.RepairTickets.Queries;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class TicketNoteQueryRepository : QueryRepository, ITicketNoteQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, RepairTicketId, Content, CreatedByUserId, CreatedAtUtc
              FROM " + TableTicketNotes;

        public TicketNoteQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<TicketNoteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<TicketNoteDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default)
            => await ListForTicketAsync(GetConnection(), repairTicketId, cancellationToken);

        // Used also by RepairTicketQueryRepository to compose the full card (single mapping rule).
        internal async Task<IReadOnlyList<TicketNoteDto>> ListForTicketAsync(
            IDbConnection connection, Guid repairTicketId, CancellationToken cancellationToken = default)
        {
            var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE RepairTicketId = @repairTicketId ORDER BY CreatedAtUtc",
                new { repairTicketId }, cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        private static TicketNoteDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            GuidOf(row, "RepairTicketId"),
            StringOf(row, "Content"),
            GuidOf(row, "CreatedByUserId"),
            DateTimeUtcOf(row, "CreatedAtUtc"));
    }
}
