using System.Data;
using Dapper;
using FixDesk.Application.RepairTickets.Dtos;
using FixDesk.Application.RepairTickets.Queries;
using FixDesk.Domain.Payments;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class PaymentQueryRepository : QueryRepository, IPaymentQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, RepairTicketId, Type, AmountInCents, OriginalPaymentId, CreatedByUserId, CreatedAtUtc
              FROM " + TablePayments;

        public PaymentQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<PaymentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<PaymentDto>> GetAllByTicketIdAsync(Guid repairTicketId, CancellationToken cancellationToken = default)
            => await ListForTicketAsync(GetConnection(), repairTicketId, cancellationToken);

        // Used also by RepairTicketQueryRepository to compose the full card (single mapping rule).
        internal async Task<IReadOnlyList<PaymentDto>> ListForTicketAsync(
            IDbConnection connection, Guid repairTicketId, CancellationToken cancellationToken = default)
        {
            var rows = await connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE RepairTicketId = @repairTicketId ORDER BY CreatedAtUtc",
                new { repairTicketId }, cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        private static PaymentDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            GuidOf(row, "RepairTicketId"),
            EnumOf<PaymentType>(row, "Type"),
            LongOf(row, "AmountInCents"),
            NullableGuidOf(row, "OriginalPaymentId"),
            GuidOf(row, "CreatedByUserId"),
            DateTimeUtcOf(row, "CreatedAtUtc"));
    }
}
