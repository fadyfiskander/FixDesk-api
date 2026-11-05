using FixDesk.Domain.Brands.Repositories;
using FixDesk.Domain.Categories.Repositories;
using FixDesk.Domain.Customers.Repositories;
using FixDesk.Domain.Devices.Repositories;
using FixDesk.Domain.Identity;
using FixDesk.Domain.Identity.Repositories;
using FixDesk.Domain.RepairTickets.Repositories;
using FixDesk.Domain.Settings.Repositories;
using FixDesk.Domain.Technicians.Repositories;
using FixDesk.Infrastructure.Persistence;
using FixDesk.Infrastructure.Repositories.Write;
using Microsoft.AspNetCore.Identity;
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

            services.AddSingleton(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

            services.AddDbContext<AppDbContext>(options =>
                options
                    .UseSqlite(connectionString)
                    .AddInterceptors(new SqlitePragmaConnectionInterceptor()));

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITechnicianRepository, TechnicianRepository>();
            services.AddScoped<IRepairTicketRepository, RepairTicketRepository>();
            services.AddScoped<IShopSettingsRepository, ShopSettingsRepository>();

            return services;
        }
    }
}
