using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FixDesk.Infrastructure.Persistence
{
    // لاستخدام `dotnet ef migrations add` بدون الـ Host كله.
    // ينفّذ أثناء الـ design-time فقط — لا شيء في الـ runtime.
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=FixDesk.db");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
