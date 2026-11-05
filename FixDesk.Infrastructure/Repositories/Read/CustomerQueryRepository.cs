using Dapper;
using FixDesk.Application.Customers.Dtos;
using FixDesk.Application.Customers.Queries;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class CustomerQueryRepository : QueryRepository, ICustomerQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, Name, PhoneNumber, SecondaryPhoneNumber, Email, Address, Notes, IsDeleted
              FROM " + TableCustomers;

        public CustomerQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE IsDeleted = 0 ORDER BY Name", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        public async Task<IReadOnlyList<CustomerSummaryDto>> SearchAsync(string term, CancellationToken cancellationToken = default)
        {
            var pattern = $"%{term.Trim()}%";
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition($@"
                SELECT Id, Name, PhoneNumber, SecondaryPhoneNumber, Email, Notes
                FROM {TableCustomers}
                WHERE IsDeleted = 0
                  AND (Name LIKE @pattern OR PhoneNumber LIKE @pattern OR SecondaryPhoneNumber LIKE @pattern)
                ORDER BY Name", new { pattern }, cancellationToken: cancellationToken));
            return rows.Select(MapSummaryRow).ToList();
        }

        private static CustomerDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "Name"),
            StringOf(row, "PhoneNumber"),
            NullableStringOf(row, "SecondaryPhoneNumber"),
            NullableStringOf(row, "Email"),
            NullableStringOf(row, "Address"),
            NullableStringOf(row, "Notes"),
            BoolOf(row, "IsDeleted"));

        private static CustomerSummaryDto MapSummaryRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "Name"),
            StringOf(row, "PhoneNumber"),
            NullableStringOf(row, "SecondaryPhoneNumber"),
            NullableStringOf(row, "Email"),
            NullableStringOf(row, "Notes"));
    }
}
