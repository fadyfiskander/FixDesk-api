using FixDesk.Domain.Identity;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Identity
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, value => new UserId(value))
                .ValueGeneratedNever();

            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(200);
            builder.Property(u => u.Role).HasConversion<int>();

            // التواريخ UTC ticks (المرجع - قسم 3)
            builder.Property(u => u.CreatedAtUtc).HasConversion(new UtcDateTimeTicksConverter());

            // الدخول بـ username — من غير حساسية حالة الحروف
            builder.Property(u => u.Username).UseCollation("NOCASE");
            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
