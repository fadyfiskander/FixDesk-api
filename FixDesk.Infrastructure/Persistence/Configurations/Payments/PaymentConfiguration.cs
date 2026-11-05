using FixDesk.Domain.Payments;
using FixDesk.Domain.RepairTickets;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Payments
{
    internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.RepairTicketId)
                .HasConversion(id => id.Value, value => new RepairTicketId(value));

            builder.Property(p => p.Type).HasConversion<int>();

            // الفلوس INTEGER بالقروش، الدفعة موجبة دايمًا (التصحيح بقيد عكسي)
            builder.Property(p => p.AmountInCents).IsRequired();

            builder.Property(p => p.CreatedAtUtc).HasConversion(new UtcDateTimeTicksConverter());

            // لإمكانية تتبّع القيد العكسي حصته من الدفعة الأصلية
            builder.HasIndex(p => p.OriginalPaymentId);
            builder.HasIndex(p => p.RepairTicketId);
        }
    }
}
