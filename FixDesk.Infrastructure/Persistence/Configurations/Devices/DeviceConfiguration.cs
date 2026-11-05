using FixDesk.Domain.Brands;
using FixDesk.Domain.Categories;
using FixDesk.Domain.Customers;
using FixDesk.Domain.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Devices
{
    internal sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.ToTable("Devices");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id)
                .HasConversion(id => id.Value, value => new DeviceId(value))
                .ValueGeneratedNever();

            // الموديل نص حر (المرجع - قسم 5)، السيريال اختياري
            builder.Property(d => d.Model).IsRequired().HasMaxLength(300);
            builder.Property(d => d.SerialNumber).HasMaxLength(200);

            builder.Property(d => d.CustomerId)
                .HasConversion(id => id.Value, value => new CustomerId(value));
            builder.Property(d => d.CategoryId)
                .HasConversion(id => id.Value, value => new CategoryId(value));
            builder.Property(d => d.BrandId)
                .HasConversion(id => id.Value, value => new BrandId(value));

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Brand>()
                .WithMany()
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.CustomerId);
            builder.HasIndex(d => d.IsDeleted);
        }
    }
}
