using FixDesk.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using FixDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FixDesk.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SQLiteConnection")
                ?? throw new InvalidOperationException("Connection string 'SQLiteConnection' is not configured.");

            // الدخول بـ username/password مع PasswordHasher بدل ASP.NET Identity كامل (المرجع - قسم 4)
            services.AddSingleton(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

            services.AddDbContext<AppDbContext>(options =>
                options
                    .UseSqlite(connectionString)
                    // إعدادات SQLite (WAL وforeign_keys وbusy_timeout وsynchronous) على كل اتصال
                    .AddInterceptors(new SqlitePragmaConnectionInterceptor()));

            return services;
        }
    }
}
