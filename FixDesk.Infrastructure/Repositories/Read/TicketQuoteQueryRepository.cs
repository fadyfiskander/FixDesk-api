using System.Data;
using Dapper;
using FixDesk.Application.RepairTickets.Dtos;
using FixDesk.Application.RepairTickets.Queries;
using FixDesk.Domain.TicketQuotes;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class TicketQuoteQueryRepository : QueryRepository, ITicketQuoteQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, RepairTicketId, VersionNumber, AmountInCents, Reason, Status,
                     CreatedAtUtc, ActionAtUtc, ActionByUserId, RejectReason
              FROM " + TableTicketQuotes;

        public TicketQuoteQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<TicketQuoteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<TicketQuoteDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default)
            => await ListForTicketAsync(GetConnection(), repairTicketId, cancellationToken);

        // Used also by RepairTicketQueryRepository to compose the full card (single mapping rule).
        internal async Task<IReadOnlyList<TicketQuoteDto>> ListForTicketAsync(
            IDbConnection connection, Guid repairTicketId, CancellationToken cancellationToken = default)
        {
            var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE RepairTicketId = @repairTicketId ORDER BY VersionNumber",
                new { repairTicketId }, cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        private static TicketQuoteDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            GuidOf(row, "RepairTicketId"),
            IntOf(row, "VersionNumber"),
            LongOf(row, "AmountInCents"),
            StringOf(row, "Reason"),
            EnumOf<QuoteStatus>(row, "Status"),
            DateTimeUtcOf(row, "CreatedAtUtc"),
            NullableDateTimeUtcOf(row, "ActionAtUtc"),
            NullableGuidOf(row, "ActionByUserId"),
            NullableStringOf(row, "RejectReason"));
    }
}
