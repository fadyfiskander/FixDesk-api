using Dapper;
using FixDesk.Application.Settings.Dtos;
using FixDesk.Application.Settings.Queries;
using FixDesk.Domain.Settings;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class ShopSettingsQueryRepository : QueryRepository, IShopSettingsQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, ShopName, PhoneNumber, Address, InspectionFeeEnabled, DefaultInspectionFeeInCents, UpdatedAtUtc
              FROM " + TableShopSettings;

        public ShopSettingsQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<ShopSettingsDto?> GetAsync(CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @singleRowId ORDER BY Id LIMIT 1",
                new { singleRowId = ShopSettings.SingleRowId },
                cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        private static ShopSettingsDto MapRow(dynamic row) => new(
            StringOf(row, "ShopName"),
            NullableStringOf(row, "PhoneNumber"),
            NullableStringOf(row, "Address"),
            BoolOf(row, "InspectionFeeEnabled"),
            NullableLongOf(row, "DefaultInspectionFeeInCents"),
            DateTimeUtcOf(row, "UpdatedAtUtc"));
    }
}
