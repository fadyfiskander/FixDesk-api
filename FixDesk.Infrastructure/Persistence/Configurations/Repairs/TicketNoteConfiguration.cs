using FixDesk.Domain.RepairTickets;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Repairs
{
    internal sealed class TicketNoteConfiguration : IEntityTypeConfiguration<TicketNote>
    {
        public void Configure(EntityTypeBuilder<TicketNote> builder)
        {
            builder.ToTable("TicketNotes");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id).ValueGeneratedNever();

            builder.Property(n => n.RepairTicketId)
                .HasConversion(id => id.Value, value => new RepairTicketId(value));

            builder.Property(n => n.Content).IsRequired().HasMaxLength(4000);
            builder.Property(n => n.CreatedAtUtc).HasConversion(new UtcDateTimeTicksConverter());

            builder.HasIndex(n => n.RepairTicketId);
        }
    }
}
