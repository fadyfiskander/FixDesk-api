using System.Data;
using Dapper;
using FixDesk.Application.RepairTickets.Dtos;
using FixDesk.Application.RepairTickets.Queries;
using FixDesk.Domain.RepairTickets;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class TicketStatusHistoryQueryRepository : QueryRepository, ITicketStatusHistoryQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, RepairTicketId, FromState, ToState, ChangedByUserId, ChangedAtUtc
              FROM " + TableTicketStatusHistory;

        public TicketStatusHistoryQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<TicketStatusHistoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<TicketStatusHistoryDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default)
            => await ListForTicketAsync(GetConnection(), repairTicketId, cancellationToken);

        // Used also by RepairTicketQueryRepository to compose the full card (single mapping rule).
        internal async Task<IReadOnlyList<TicketStatusHistoryDto>> ListForTicketAsync(
            IDbConnection connection, Guid repairTicketId, CancellationToken cancellationToken = default)
        {
            // Audit ordering: time first; equal timestamps follow the state progression
            // (the enum numbers are ordered as the lifecycle advances).
            var rows = await connection.QueryAsync<dynamic>(new CommandDefinition($@"
                SELECT Id, RepairTicketId, FromState, ToState, ChangedByUserId, ChangedAtUtc
                FROM {TableTicketStatusHistory}
                WHERE RepairTicketId = @repairTicketId
                ORDER BY ChangedAtUtc, ToState",
                new { repairTicketId }, cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        private static TicketStatusHistoryDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            GuidOf(row, "RepairTicketId"),
            NullableEnumOf<RepairTicketState>(row, "FromState"),
            EnumOf<RepairTicketState>(row, "ToState"),
            GuidOf(row, "ChangedByUserId"),
            DateTimeUtcOf(row, "ChangedAtUtc"));
    }
}
