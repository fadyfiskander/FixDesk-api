using FixDesk.Domain.RepairTickets;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Repairs
{
    internal sealed class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
    {
        public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
        {
            builder.ToTable("TicketStatusHistory");

            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id).ValueGeneratedNever();

            builder.Property(h => h.RepairTicketId)
                .HasConversion(id => id.Value, value => new RepairTicketId(value));

            builder.Property(h => h.FromState).HasConversion<int?>();
            builder.Property(h => h.ToState).HasConversion<int>();
            builder.Property(h => h.ChangedAtUtc).HasConversion(new UtcDateTimeTicksConverter());

            builder.HasIndex(h => h.RepairTicketId);
        }
    }
}
