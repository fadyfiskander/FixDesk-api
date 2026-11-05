using FixDesk.Domain.Settings;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Settings
{
    // صف واحد فقط بمفتاح ثابت (SingleRowId).
    internal sealed class ShopSettingsConfiguration : IEntityTypeConfiguration<ShopSettings>
    {
        public void Configure(EntityTypeBuilder<ShopSettings> builder)
        {
            builder.ToTable("ShopSettings");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.Property(s => s.ShopName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.PhoneNumber).HasMaxLength(50);
            builder.Property(s => s.Address).HasMaxLength(300);
            // الفلوس INTEGER بالقروش
            builder.Property(s => s.DefaultInspectionFeeInCents);

            // التواريخ UTC ticks (المرجع - قسم 3)
            builder.Property(s => s.UpdatedAtUtc).HasConversion(new UtcDateTimeTicksConverter());
        }
    }
}
