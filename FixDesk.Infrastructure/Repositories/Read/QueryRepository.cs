using System.Data;
using System.Data.Common;
using System.Globalization;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    // Base of every Dapper read repository. It manages everything once:
    // the connection, the table names, and the SQLite row-conversion rules
    // (GUID as TEXT, DateTime as UTC ticks, bool/enum as INTEGER).
    // The DI container owns every repository instance per scope and disposes
    // it (with its connection) when the scope ends.
    public abstract class QueryRepository : IDisposable
    {
        protected readonly DbConnection Connection;

        protected QueryRepository(ISqliteConnectionFactory connectionFactory)
        {
            Connection = connectionFactory.CreateOpenConnection();
        }

        protected IDbConnection GetConnection() => Connection;

        protected const string TableCustomers = "Customers";
        protected const string TableDevices = "Devices";
        protected const string TableBrands = "Brands";
        protected const string TableCategories = "Categories";
        protected const string TableUsers = "Users";
        protected const string TableTechnicians = "Technicians";
        protected const string TableShopSettings = "ShopSettings";
        protected const string TableRepairTickets = "RepairTickets";
        protected const string TableTicketQuotes = "TicketQuotes";
        protected const string TablePayments = "Payments";
        protected const string TableTicketNotes = "TicketNotes";
        protected const string TableTicketStatusHistory = "TicketStatusHistory";

        public void Dispose() => Connection.Dispose();

        private static object? Raw(dynamic row, string column)
        {
            var dictionary = (IDictionary<string, object?>)row;
            return dictionary.TryGetValue(column, out var value) ? value : null;
        }

        protected static Guid GuidOf(dynamic row, string column)
        {
            var value = Raw(row, column)
                ?? throw new InvalidOperationException($"Column '{column}' returned null where a GUID is required.");

            return value switch
            {
                Guid guid => guid,
                string text => Guid.Parse(text),
                _ => throw new InvalidOperationException($"Column '{column}' is not a valid GUID value.")
            };
        }

        protected static Guid? NullableGuidOf(dynamic row, string column)
        {
            var value = Raw(row, column);
            if (value is null) return null;
            return value switch
            {
                Guid guid => guid,
                string text => Guid.Parse(text),
                _ => throw new InvalidOperationException($"Column '{column}' is not a valid GUID value.")
            };
        }

        protected static string StringOf(dynamic row, string column)
            => (string?)Raw(row, column)
                ?? throw new InvalidOperationException($"Column '{column}' returned null where a string is required.");

        protected static string? NullableStringOf(dynamic row, string column)
            => (string?)Raw(row, column);

        protected static long LongOf(dynamic row, string column)
        {
            var value = Raw(row, column)
                ?? throw new InvalidOperationException($"Column '{column}' returned null where a number is required.");
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        }

        protected static long? NullableLongOf(dynamic row, string column)
            => Raw(row, column) is null
                ? null
                : Convert.ToInt64(Raw(row, column), CultureInfo.InvariantCulture);

        protected static int IntOf(dynamic row, string column)
        {
            var value = Raw(row, column)
                ?? throw new InvalidOperationException($"Column '{column}' returned null where a number is required.");
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        protected static int? NullableIntOf(dynamic row, string column)
            => Raw(row, column) is null
                ? null
                : Convert.ToInt32(Raw(row, column), CultureInfo.InvariantCulture);

        // SQLite stores booleans as 0/1 INTEGER.
        protected static bool BoolOf(dynamic row, string column) => LongOf(row, column) != 0L;

        // All timestamps are stored as UTC ticks.
        protected static DateTime DateTimeUtcOf(dynamic row, string column)
            => new DateTime(LongOf(row, column), DateTimeKind.Utc);

        protected static DateTime? NullableDateTimeUtcOf(dynamic row, string column)
            => Raw(row, column) is null
                ? null
                : new DateTime(LongOf(row, column), DateTimeKind.Utc);

        // SQLite stores enum members as INTEGER.
        protected static TEnum EnumOf<TEnum>(dynamic row, string column) where TEnum : struct, Enum
            => (TEnum)Enum.ToObject(typeof(TEnum), LongOf(row, column));

        protected static TEnum? NullableEnumOf<TEnum>(dynamic row, string column) where TEnum : struct, Enum
            => Raw(row, column) is null
                ? null
                : (TEnum)Enum.ToObject(typeof(TEnum), LongOf(row, column));
    }
}
