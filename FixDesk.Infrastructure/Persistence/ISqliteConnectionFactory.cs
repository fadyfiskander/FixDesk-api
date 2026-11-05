using System.Data.Common;

namespace FixDesk.Infrastructure.Persistence
{
    // One place that knows how to open SQLite read connections (file path, settings).
    public interface ISqliteConnectionFactory
    {
        DbConnection CreateOpenConnection();
    }
}
