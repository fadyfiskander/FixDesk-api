using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Brands;
using FixDesk.Domain.Categories;
using FixDesk.Domain.Customers;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Devices
{
    // الجهاز: Category + Brand + الموديل (نص حر) + السيريال (اختياري) + العميل (المرجع - قسم 5).
    public class Device : AggregateRoot<DeviceId>
    {
        public string Model { get; private set; }
        public string? SerialNumber { get; private set; }
        public CategoryId CategoryId { get; private set; }
        public BrandId BrandId { get; private set; }
        public CustomerId CustomerId { get; private set; }

        // Soft Delete: للعملاء والأجهزة (المرجع - قسم 5).
        public bool IsDeleted { get; private set; }

        [SetsRequiredMembers]
        private Device(string model, BrandId brandId, CategoryId categoryId, CustomerId customerId, string? serialNumber)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Device model cannot be null or empty.", nameof(model));

            Id = DeviceId.New();
            Model = model.Trim();
            SerialNumber = TrimOrNull(serialNumber);
            BrandId = brandId;
            CategoryId = categoryId;
            CustomerId = customerId;
            IsDeleted = false;
        }

        public static Device Create(string model, BrandId brandId, CategoryId categoryId, CustomerId customerId, string? serialNumber = null)
        {
            return new Device(model, brandId, categoryId, customerId, serialNumber);
        }

        public void UpdateModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Device model cannot be null or empty.", nameof(model));
            Model = model.Trim();
        }

        public void UpdateSerialNumber(string? serialNumber)
        {
            SerialNumber = TrimOrNull(serialNumber);
        }

        public void UpdateBrand(BrandId brandId)
        {
            BrandId = brandId;
        }

        public void UpdateCategory(CategoryId categoryId)
        {
            CategoryId = categoryId;
        }

        public void UpdateCustomer(CustomerId customerId)
        {
            CustomerId = customerId;
        }

        public void Update(string model, BrandId brandId, CategoryId categoryId, CustomerId customerId, string? serialNumber = null)
        {
            UpdateModel(model);
            UpdateBrand(brandId);
            UpdateCategory(categoryId);
            UpdateCustomer(customerId);
            UpdateSerialNumber(serialNumber);
        }

        public void SoftDelete()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        private static string? TrimOrNull(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}
