using FixDesk.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Customers
{
    internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => new CustomerId(value))
                .ValueGeneratedNever();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(50);
            builder.Property(c => c.SecondaryPhoneNumber).HasMaxLength(50);
            builder.Property(c => c.Email).HasMaxLength(200);
            builder.Property(c => c.Address).HasMaxLength(300);
            builder.Property(c => c.Notes).HasMaxLength(2000);

            // Soft Delete (المرجع - قسم 5)
            builder.HasIndex(c => c.IsDeleted);
            // البحث السريع بالموبايل وقت فتح التذكرة
            builder.HasIndex(c => c.PhoneNumber);
        }
    }
}
