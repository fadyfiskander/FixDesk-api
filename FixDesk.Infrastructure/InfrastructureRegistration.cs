using FixDesk.Application.Brands.Queries;
using FixDesk.Application.Categories.Queries;
using FixDesk.Application.Customers.Queries;
using FixDesk.Application.Devices.Queries;
using FixDesk.Application.Identity.Queries;
using FixDesk.Application.RepairTickets.Queries;
using FixDesk.Application.Settings.Queries;
using FixDesk.Application.Technicians.Queries;
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
using FixDesk.Infrastructure.Repositories.Read;
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
            services.AddSingleton<ISqliteConnectionFactory>(new SqliteConnectionFactory(connectionString));

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

            // ===== Read side (Dapper -> DTOs of the Application layer) =====
            services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
            services.AddScoped<IDeviceQueryRepository, DeviceQueryRepository>();
            services.AddScoped<IBrandQueryRepository, BrandQueryRepository>();
            services.AddScoped<ICategoryQueryRepository, CategoryQueryRepository>();
            services.AddScoped<IUserQueryRepository, UserQueryRepository>();
            services.AddScoped<ITechnicianQueryRepository, TechnicianQueryRepository>();
            services.AddScoped<IShopSettingsQueryRepository, ShopSettingsQueryRepository>();
            services.AddScoped<IRepairTicketQueryRepository, RepairTicketQueryRepository>();
            services.AddScoped<ITicketQuoteQueryRepository, TicketQuoteQueryRepository>();
            services.AddScoped<IPaymentQueryRepository, PaymentQueryRepository>();
            services.AddScoped<ITicketNoteQueryRepository, TicketNoteQueryRepository>();
            services.AddScoped<ITicketStatusHistoryQueryRepository, TicketStatusHistoryQueryRepository>();

            return services;
        }
    }
}
