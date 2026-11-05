using FixDesk.Domain.Brands;
using FixDesk.Domain.Categories;
using FixDesk.Domain.Customers;
using FixDesk.Domain.Devices;
using FixDesk.Domain.Identity;
using FixDesk.Domain.RepairTickets;
using FixDesk.Domain.Settings;
using FixDesk.Domain.Technicians;
using Microsoft.EntityFrameworkCore;

namespace FixDesk.Infrastructure.Persistence
{
    // DbContext واحد (المرجع - قسم 2)، SQLite ملف واحد.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<RepairTicket> RepairTickets => Set<RepairTicket>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Technician> Technicians => Set<Technician>();
        public DbSet<ShopSettings> ShopSettings => Set<ShopSettings>();
    }
}
