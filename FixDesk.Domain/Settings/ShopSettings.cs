using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Settings
{
    // إعدادات المحل — صف واحد فقط، وثابت بمفتاح معروف (SingleRowId).
    // تغيير الإعدادات لا يأثر على التذاكر القديمة (قيم رسوم الكشف بتتجمّد وقت فتح التذكرة).
    public class ShopSettings : Entity<Guid>
    {
        public static readonly Guid SingleRowId = new("11111111-1111-1111-1111-111111111111");

        public string ShopName { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }
        public string? Address { get; private set; }

        // رسوم الكشف اختيارية حسب المحل (المرجع - قسم 9)
        public bool InspectionFeeEnabled { get; private set; }
        public long? DefaultInspectionFeeInCents { get; private set; }

        public Guid? UpdatedByUserId { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private ShopSettings()
        {
        }

        [SetsRequiredMembers]
        private ShopSettings(
            string shopName,
            string? phoneNumber,
            string? address,
            bool inspectionFeeEnabled,
            long? defaultInspectionFeeInCents,
            Guid? updatedByUserId,
            DateTime updatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(shopName))
                throw new ArgumentException("Shop name cannot be null or empty.", nameof(shopName));
            if (defaultInspectionFeeInCents is { } fee && fee < 0)
                throw new ArgumentException("Default inspection fee cannot be negative.", nameof(defaultInspectionFeeInCents));

            Id = SingleRowId;
            ShopName = shopName.Trim();
            PhoneNumber = TrimOrNull(phoneNumber);
            Address = TrimOrNull(address);
            InspectionFeeEnabled = inspectionFeeEnabled;
            // لو الرسوم مقفولة، القيمة null (المرجع - قسم 9)
            DefaultInspectionFeeInCents = inspectionFeeEnabled ? defaultInspectionFeeInCents : null;
            UpdatedByUserId = updatedByUserId;
            UpdatedAtUtc = updatedAtUtc;
        }

        public static ShopSettings CreateDefault(
            string shopName,
            bool inspectionFeeEnabled = false,
            long? defaultInspectionFeeInCents = null,
            Guid? updatedByUserId = null,
            DateTime? updatedAtUtc = null)
        {
            return new ShopSettings(
                shopName, null, null,
                inspectionFeeEnabled, defaultInspectionFeeInCents,
                updatedByUserId, updatedAtUtc ?? DateTime.UtcNow);
        }

        public void Update(
            string shopName,
            string? phoneNumber,
            string? address,
            bool inspectionFeeEnabled,
            long? defaultInspectionFeeInCents,
            Guid? updatedByUserId,
            DateTime updatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(shopName))
                throw new ArgumentException("Shop name cannot be null or empty.", nameof(shopName));
            if (defaultInspectionFeeInCents is { } fee && fee < 0)
                throw new ArgumentException("Default inspection fee cannot be negative.", nameof(defaultInspectionFeeInCents));

            ShopName = shopName.Trim();
            PhoneNumber = TrimOrNull(phoneNumber);
            Address = TrimOrNull(address);
            InspectionFeeEnabled = inspectionFeeEnabled;
            DefaultInspectionFeeInCents = inspectionFeeEnabled ? defaultInspectionFeeInCents : null;
            UpdatedByUserId = updatedByUserId;
            UpdatedAtUtc = updatedAtUtc;
        }

        private static string? TrimOrNull(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}
