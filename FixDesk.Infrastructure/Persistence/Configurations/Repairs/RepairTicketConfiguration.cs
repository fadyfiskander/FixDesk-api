using FixDesk.Domain.Brands;
using FixDesk.Domain.Categories;
using FixDesk.Domain.Customers;
using FixDesk.Domain.Devices;
using FixDesk.Domain.RepairTickets;
using FixDesk.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixDesk.Infrastructure.Persistence.Configurations.Repairs
{
    internal sealed class RepairTicketConfiguration : IEntityTypeConfiguration<RepairTicket>
    {
        public void Configure(EntityTypeBuilder<RepairTicket> builder)
        {
            builder.ToTable("RepairTickets");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasConversion(id => id.Value, value => new RepairTicketId(value))
                .ValueGeneratedNever();

            // الرقم المقروء فريد (FD-000123)
            builder.Property(t => t.TicketNumber).IsRequired();
            builder.HasIndex(t => t.TicketNumber).IsUnique();

            builder.Property(t => t.CustomerId)
                .HasConversion(id => id.Value, value => new CustomerId(value));
            builder.Property(t => t.DeviceId)
                .HasConversion(id => id.Value, value => new DeviceId(value));

            builder.Property(t => t.ReportedIssue).IsRequired().HasMaxLength(2000);
            builder.Property(t => t.Diagnosis).HasMaxLength(4000);

            // الفلوس INTEGER بالقروش
            builder.Property(t => t.InspectionFeeInCents);

            builder.Property(t => t.TicketState).HasConversion<int>();
            builder.Property(t => t.CreatedAtUtc).HasConversion(new UtcDateTimeTicksConverter());
            builder.Property(t => t.ExpectedDeliveryAtUtc).HasConversion(new NullableUtcDateTimeTicksConverter());
            builder.Property(t => t.WarrantyStartDateUtc).HasConversion(new NullableUtcDateTimeTicksConverter());

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Device>()
                .WithMany()
                .HasForeignKey(t => t.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            // الفني (اختياري) — إشارة بقيمة إلى Technicians.Id (المرجع - قسم 4)،
            // من غير علاقة FK عشان تتوافق مع الـ Strongly-typed Id.
            builder.HasIndex(t => t.AssignedTechnicianId);

            // كائنات القيمة: JSON داخل عمود واحد
            builder.Property(t => t.ReceptionCondition)
                .HasConversion(new DeviceConditionJsonConverter());
            builder.Property(t => t.ReceptionCondition)
                .Metadata.SetValueComparer(new DeviceConditionComparer());

            builder.Property(t => t.Accessories)
                .HasField("_accessories")
                .HasConversion(new DeviceAttachmentsJsonConverter());
            builder.Property(t => t.Accessories)
                .Metadata.SetValueComparer(new DeviceAttachmentsComparer());

            // ===== children (تحت هذا الـ Aggregate) =====
            builder.HasMany(t => t.Quotes)
                .WithOne()
                .HasForeignKey(q => q.RepairTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.StatusHistory)
                .WithOne()
                .HasForeignKey(h => h.RepairTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Notes)
                .WithOne()
                .HasForeignKey(n => n.RepairTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Payments)
                .WithOne()
                .HasForeignKey(p => p.RepairTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // الـ Collections محمية (IReadOnlyCollection + backing fields) — EF يتعامل مع الـ fields
            builder.Navigation(t => t.Quotes).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(t => t.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(t => t.Notes).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(t => t.Payments).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(t => t.CustomerId);
            builder.HasIndex(t => t.DeviceId);
            builder.HasIndex(t => t.TicketState);
        }
    }
}
