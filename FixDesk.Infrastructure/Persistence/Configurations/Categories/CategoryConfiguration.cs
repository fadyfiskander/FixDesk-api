using FixDesk.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Categories
{
    internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => new CategoryId(value))
                .ValueGeneratedNever();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);

            // Unique على الاسم من غير حساسية لحالة الحروف (المرجع - قسم 5)
            builder.Property(c => c.Name).UseCollation("NOCASE");
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
