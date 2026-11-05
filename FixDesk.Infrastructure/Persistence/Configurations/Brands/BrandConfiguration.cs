using FixDesk.Domain.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Brands
{
    internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasConversion(id => id.Value, value => new BrandId(value))
                .ValueGeneratedNever();

            builder.Property(b => b.Name).IsRequired().HasMaxLength(200);

            // Unique على الاسم من غير حساسية لحالة الحروف (المرجع - قسم 5)
            builder.Property(b => b.Name).UseCollation("NOCASE");
            builder.HasIndex(b => b.Name).IsUnique();
        }
    }
}
