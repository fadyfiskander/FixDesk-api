using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FixDesk.Infrastructure.Persistence
{
    // Opens forward-only SQLite connections for the Dapper read side.
    public sealed class SqliteConnectionFactory(string connectionString) : ISqliteConnectionFactory
    {
        public DbConnection CreateOpenConnection()
        {
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
