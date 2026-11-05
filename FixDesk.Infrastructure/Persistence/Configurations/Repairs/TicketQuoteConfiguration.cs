using FixDesk.Domain.RepairTickets;
using FixDesk.Domain.TicketQuotes;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Repairs
{
    internal sealed class TicketQuoteConfiguration : IEntityTypeConfiguration<TicketQuote>
    {
        public void Configure(EntityTypeBuilder<TicketQuote> builder)
        {
            builder.ToTable("TicketQuotes");

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                .HasConversion(id => id.Value, value => new TicketQuoteId(value))
                .ValueGeneratedNever();

            builder.Property(q => q.RepairTicketId)
                .HasConversion(id => id.Value, value => new RepairTicketId(value));

            // الفلوس INTEGER بالقروش
            builder.Property(q => q.AmountInCents).IsRequired();

            builder.Property(q => q.Reason).IsRequired().HasMaxLength(1000);
            builder.Property(q => q.RejectReason).HasMaxLength(1000);
            builder.Property(q => q.Status).HasConversion<int>();
            builder.Property(q => q.CreatedAtUtc).HasConversion(new UtcDateTimeTicksConverter());
            builder.Property(q => q.ActionAtUtc).HasConversion(new NullableUtcDateTimeTicksConverter());

            builder.Property(q => q.ActionByUserId);

            // نسخة واحدة لكل رقم على نفس التذكرة
            builder.HasIndex(q => new { q.RepairTicketId, q.VersionNumber }).IsUnique();
            builder.HasIndex(q => q.Status);
        }
    }
}
