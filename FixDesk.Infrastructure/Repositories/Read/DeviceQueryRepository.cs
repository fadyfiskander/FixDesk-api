using Dapper;
using FixDesk.Application.Devices.Dtos;
using FixDesk.Application.Devices.Queries;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class DeviceQueryRepository : QueryRepository, IDeviceQueryRepository
    {
        // Display names come with the row: no extra round-trips for the screens.
        private const string BaseSelect =
            @"SELECT d.Id, d.Model, d.SerialNumber,
                     d.CategoryId, c.Name AS CategoryName,
                     d.BrandId, b.Name AS BrandName,
                     d.CustomerId, cu.Name AS CustomerName,
                     d.IsDeleted
              FROM " + TableDevices + @" d
              JOIN " + TableCategories + @" c ON c.Id = d.CategoryId
              JOIN " + TableBrands + @" b ON b.Id = d.BrandId
              JOIN " + TableCustomers + @" cu ON cu.Id = d.CustomerId";

        public DeviceQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<DeviceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE d.Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<DeviceDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE d.IsDeleted = 0 ORDER BY d.Model", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        public async Task<IReadOnlyList<DeviceSummaryDto>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition($@"
                SELECT d.Id, d.Model, d.SerialNumber,
                       b.Name AS BrandName, c.Name AS CategoryName,
                       d.CustomerId, cu.Name AS CustomerName
                FROM {TableDevices} d
                JOIN {TableCategories} c ON c.Id = d.CategoryId
                JOIN {TableBrands} b ON b.Id = d.BrandId
                JOIN {TableCustomers} cu ON cu.Id = d.CustomerId
                WHERE d.IsDeleted = 0 AND d.CustomerId = @customerId
                ORDER BY d.Model", new { customerId }, cancellationToken: cancellationToken));
            return rows.Select(MapSummaryRow).ToList();
        }

        private static DeviceDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "Model"),
            NullableStringOf(row, "SerialNumber"),
            GuidOf(row, "CategoryId"),
            StringOf(row, "CategoryName"),
            GuidOf(row, "BrandId"),
            StringOf(row, "BrandName"),
            GuidOf(row, "CustomerId"),
            StringOf(row, "CustomerName"),
            BoolOf(row, "IsDeleted"));

        private static DeviceSummaryDto MapSummaryRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "Model"),
            NullableStringOf(row, "SerialNumber"),
            StringOf(row, "BrandName"),
            StringOf(row, "CategoryName"),
            GuidOf(row, "CustomerId"),
            StringOf(row, "CustomerName"));
    }
}
