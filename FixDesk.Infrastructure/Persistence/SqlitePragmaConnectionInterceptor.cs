using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FixDesk.Infrastructure.Persistence
{
    // إعدادات SQLite (المرجع - قسم 3) تُطبّق على كل اتصال جديد:
    // journal_mode=WAL (بي persists في ملف الداتابيز)، foreign_keys=ON وbusy_timeout وsynchronous لكل اتصال.
    public sealed class SqlitePragmaConnectionInterceptor : DbConnectionInterceptor
    {
        private const string Pragmas =
            "PRAGMA journal_mode=WAL;" +
            "PRAGMA foreign_keys=ON;" +
            "PRAGMA busy_timeout=5000;" +
            "PRAGMA synchronous=NORMAL;";

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            Apply((SqliteConnection)connection);
        }

        public override async Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            Apply((SqliteConnection)connection);
            await Task.CompletedTask;
        }

        private static void Apply(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = Pragmas;
            command.ExecuteNonQuery();
        }
    }
}
