using System.Data;
using System.Text.Json;
using Dapper;
using FixDesk.Application.RepairTickets.Dtos;
using FixDesk.Application.RepairTickets.Queries;
using FixDesk.Domain.Payments;
using FixDesk.Domain.RepairTickets;
using FixDesk.Domain.TicketQuotes;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class RepairTicketQueryRepository : QueryRepository, IRepairTicketQueryRepository
    {
        private readonly TicketQuoteQueryRepository _quotes;
        private readonly PaymentQueryRepository _payments;
        private readonly TicketNoteQueryRepository _notes;
        private readonly TicketStatusHistoryQueryRepository _history;

        public RepairTicketQueryRepository(
            ISqliteConnectionFactory connectionFactory,
            TicketQuoteQueryRepository quotes,
            PaymentQueryRepository payments,
            TicketNoteQueryRepository notes,
            TicketStatusHistoryQueryRepository history)
            : base(connectionFactory)
        {
            _quotes = quotes;
            _payments = payments;
            _notes = notes;
            _history = history;
        }

        // Money rule of the Domain (section 9), expressed once here for the read side:
        // approved quote -> its amount; rejected ticket -> min(inspection fee, latest amount); otherwise the fee.
        private const string DueAmountSql = @"
            CASE
                WHEN x.ApprovedAmount IS NOT NULL THEN x.ApprovedAmount
                WHEN x.TicketState = @rejectedState AND x.LatestAmount IS NOT NULL
                     THEN min(COALESCE(x.InspectionFeeInCents, 0), x.LatestAmount)
                ELSE COALESCE(x.InspectionFeeInCents, 0)
            END";

        private const string RootSourceSql =
            @"SELECT t.Id, t.TicketNumber, t.TicketState, t.CreatedAtUtc, t.ExpectedDeliveryAtUtc,
                   t.CustomerId, t.DeviceId,
                   t.ReportedIssue, t.Accessories, t.ReceptionCondition, t.InspectionFeeInCents,
                   t.Diagnosis, t.AssignedTechnicianId, t.WarrantyDays, t.WarrantyStartDateUtc,
                   (SELECT q.AmountInCents
                      FROM " + TableTicketQuotes + @" q
                     WHERE q.RepairTicketId = t.Id AND q.Status = @approvedStatus
                     ORDER BY q.VersionNumber DESC
                     LIMIT 1) AS ApprovedAmount,
                   (SELECT qq.AmountInCents
                      FROM " + TableTicketQuotes + @" qq
                     WHERE qq.RepairTicketId = t.Id
                     ORDER BY qq.VersionNumber DESC
                     LIMIT 1) AS LatestAmount,
                   (SELECT COALESCE(SUM(CASE p.Type
                          WHEN @paymentType THEN p.AmountInCents
                          WHEN @reversalType THEN -p.AmountInCents END), 0)
                      FROM " + TablePayments + @" p
                     WHERE p.RepairTicketId = t.Id) AS TotalPaidCents
            FROM " + TableRepairTickets + " t";

        private const string JoinedTablesSql =
            @"  JOIN " + TableCustomers + @" cu ON cu.Id = x.CustomerId
              JOIN " + TableDevices + @" dv ON dv.Id = x.DeviceId";

        private const string DetailColumnsSql =
            @"SELECT x.Id, x.TicketNumber, x.TicketState, x.CreatedAtUtc, x.ExpectedDeliveryAtUtc,
                     cu.Id AS CustomerId, cu.Name AS CustomerName, cu.PhoneNumber AS CustomerPhoneNumber,
                     dv.Id AS DeviceId, dv.Model AS DeviceModel, dv.SerialNumber AS DeviceSerialNumber,
                     x.ReportedIssue, x.Accessories, x.ReceptionCondition, x.InspectionFeeInCents,
                     x.Diagnosis, x.AssignedTechnicianId, x.WarrantyDays, x.WarrantyStartDateUtc,
                     " + DueAmountSql + @" AS DueAmountInCents,
                     x.TotalPaidCents AS TotalPaidInCents,
                     (" + DueAmountSql + @") - x.TotalPaidCents AS RemainingAmountInCents";

        private const string SummaryColumnsSql =
            @"SELECT x.Id, x.TicketNumber, x.TicketState, x.CreatedAtUtc, x.ExpectedDeliveryAtUtc,
                     cu.Id AS CustomerId, cu.Name AS CustomerName, cu.PhoneNumber AS CustomerPhoneNumber,
                     dv.Id AS DeviceId, dv.Model AS DeviceModel,
                     " + DueAmountSql + @" AS DueAmountInCents,
                     x.TotalPaidCents AS TotalPaidInCents,
                     (" + DueAmountSql + @") - x.TotalPaidCents AS RemainingAmountInCents";

        private const string DetailSelectSql = DetailColumnsSql + @" FROM (" + RootSourceSql + @") x" + JoinedTablesSql;

        private const string SummarySelectSql = SummaryColumnsSql + @" FROM (" + RootSourceSql + @") x" + JoinedTablesSql;

        // Empty children until the ticket detail composes with their own repositories.
        private static readonly IReadOnlyList<TicketQuoteDto> NoQuotes = [];
        private static readonly IReadOnlyList<PaymentDto> NoPayments = [];
        private static readonly IReadOnlyList<TicketNoteDto> NoNotes = [];
        private static readonly IReadOnlyList<TicketStatusHistoryDto> NoHistory = [];

        private static readonly JsonSerializerOptions ReadJsonOptions = new(JsonSerializerDefaults.Web);

        public async Task<RepairTicketDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var parameters = BaseTicketParameters();
            parameters.Add("id", id);

            var root = await MapRootAsync(GetConnection(), " WHERE x.Id = @id", parameters, cancellationToken);
            if (root is null) return null;

            var quotes = await _quotes.ListForTicketAsync(GetConnection(), id, cancellationToken);
            var payments = await _payments.ListForTicketAsync(GetConnection(), id, cancellationToken);
            var notes = await _notes.ListForTicketAsync(GetConnection(), id, cancellationToken);
            var history = await _history.ListForTicketAsync(GetConnection(), id, cancellationToken);

            return root with { Quotes = quotes, Payments = payments, Notes = notes, StatusHistory = history };
        }

        public async Task<RepairTicketDto?> GetByTicketNumberAsync(long ticketNumber, CancellationToken cancellationToken = default)
        {
            var parameters = BaseTicketParameters();
            parameters.Add("ticketNumber", ticketNumber);

            var root = await MapRootAsync(GetConnection(), " WHERE x.TicketNumber = @ticketNumber", parameters, cancellationToken);
            if (root is null) return null;

            var quotes = await _quotes.ListForTicketAsync(GetConnection(), root.Id, cancellationToken);
            var payments = await _payments.ListForTicketAsync(GetConnection(), root.Id, cancellationToken);
            var notes = await _notes.ListForTicketAsync(GetConnection(), root.Id, cancellationToken);
            var history = await _history.ListForTicketAsync(GetConnection(), root.Id, cancellationToken);

            return root with { Quotes = quotes, Payments = payments, Notes = notes, StatusHistory = history };
        }

        public async Task<IReadOnlyList<RepairTicketSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                SummarySelectSql + " ORDER BY x.CreatedAtUtc DESC",
                BaseTicketParameters(),
                cancellationToken: cancellationToken));
            return rows.Select(MapSummaryRow).ToList();
        }

        public async Task<IReadOnlyList<RepairTicketSummaryDto>> GetAllByCustomerIdAsync(
            Guid customerId, CancellationToken cancellationToken = default)
        {
            var parameters = BaseTicketParameters();
            parameters.Add("customerId", customerId);

            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                SummarySelectSql + " WHERE cu.Id = @customerId ORDER BY x.CreatedAtUtc DESC",
                parameters,
                cancellationToken: cancellationToken));
            return rows.Select(MapSummaryRow).ToList();
        }

        private async Task<RepairTicketDto?> MapRootAsync(
            IDbConnection connection, string whereSql, object parameters, CancellationToken cancellationToken)
        {
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                DetailSelectSql + whereSql, parameters, cancellationToken: cancellationToken));
            return row is null ? null : MapRootRow(row);
        }

        // One shared parameter set: Dapper ignores parameters a particular query never references.
        private static DynamicParameters BaseTicketParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("approvedStatus", (int)QuoteStatus.Approved);
            parameters.Add("rejectedState", (int)RepairTicketState.Rejected);
            parameters.Add("paymentType", (int)PaymentType.Payment);
            parameters.Add("reversalType", (int)PaymentType.Reversal);
            return parameters;
        }

        private static RepairTicketDto MapRootRow(dynamic row) => new(
            GuidOf(row, "Id"),
            LongOf(row, "TicketNumber"),
            EnumOf<RepairTicketState>(row, "TicketState"),
            DateTimeUtcOf(row, "CreatedAtUtc"),
            NullableDateTimeUtcOf(row, "ExpectedDeliveryAtUtc"),
            GuidOf(row, "CustomerId"),
            StringOf(row, "CustomerName"),
            StringOf(row, "CustomerPhoneNumber"),
            GuidOf(row, "DeviceId"),
            StringOf(row, "DeviceModel"),
            NullableStringOf(row, "DeviceSerialNumber"),
            StringOf(row, "ReportedIssue"),
            ParseCondition(StringOf(row, "ReceptionCondition")),
            ParseAccessories(NullableStringOf(row, "Accessories")),
            NullableLongOf(row, "InspectionFeeInCents"),
            NullableStringOf(row, "Diagnosis"),
            NullableGuidOf(row, "AssignedTechnicianId"),
            NullableIntOf(row, "WarrantyDays"),
            NullableDateTimeUtcOf(row, "WarrantyStartDateUtc"),
            LongOf(row, "DueAmountInCents"),
            LongOf(row, "TotalPaidInCents"),
            LongOf(row, "RemainingAmountInCents"),
            NoQuotes, NoPayments, NoNotes, NoHistory);

        private static RepairTicketSummaryDto MapSummaryRow(dynamic row) => new(
            GuidOf(row, "Id"),
            LongOf(row, "TicketNumber"),
            EnumOf<RepairTicketState>(row, "TicketState"),
            DateTimeUtcOf(row, "CreatedAtUtc"),
            NullableDateTimeUtcOf(row, "ExpectedDeliveryAtUtc"),
            GuidOf(row, "CustomerId"),
            StringOf(row, "CustomerName"),
            StringOf(row, "CustomerPhoneNumber"),
            GuidOf(row, "DeviceId"),
            StringOf(row, "DeviceModel"),
            LongOf(row, "DueAmountInCents"),
            LongOf(row, "TotalPaidInCents"),
            LongOf(row, "RemainingAmountInCents"));

        private static DeviceConditionDto ParseCondition(string json)
            => JsonSerializer.Deserialize<DeviceConditionDto>(json, ReadJsonOptions)
                ?? throw new InvalidOperationException("Empty device condition JSON.");

        private static IReadOnlyList<Domain.RepairTickets.DeviceAttachment> ParseAccessories(string? json)
            => string.IsNullOrEmpty(json)
                ? []
                : JsonSerializer.Deserialize<List<Domain.RepairTickets.DeviceAttachment>>(json, ReadJsonOptions) ?? [];
    }
}
