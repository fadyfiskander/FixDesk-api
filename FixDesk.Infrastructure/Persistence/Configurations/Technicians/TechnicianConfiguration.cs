using FixDesk.Domain.Technicians;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Technicians
{
    internal sealed class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
    {
        public void Configure(EntityTypeBuilder<Technician> builder)
        {
            builder.ToTable("Technicians");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasConversion(id => id.Value, value => new TechnicianId(value))
                .ValueGeneratedNever();

            builder.Property(t => t.FullName).IsRequired().HasMaxLength(200);
            builder.Property(t => t.PhoneNumber).HasMaxLength(50);

            // UserId اختياري: الفني ممكن مالوش حساب (المرجع - قسم 4)
            builder.Property(t => t.UserId).HasConversion(new NullableUserIdConverter());
            builder.HasIndex(t => t.UserId);
        }
    }
}
