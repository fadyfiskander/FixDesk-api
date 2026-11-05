using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Customers
{
    // العميل: الاسم والموبايل إلزاميان، والباقي اختياري (المرجع - قسم 5).
    public class Customer : AggregateRoot<CustomerId>
    {
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? SecondaryPhoneNumber { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public string? Notes { get; private set; }

        // Soft Delete: للعملاء والأجهزة (المرجع - قسم 5)، مش حذف فعلي.
        public bool IsDeleted { get; private set; }

        [SetsRequiredMembers]
        private Customer(string name, string phoneNumber, string? secondaryPhoneNumber, string? email, string? address, string? notes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Customer phone number cannot be null or empty.", nameof(phoneNumber));

            Id = CustomerId.New();
            Name = name.Trim();
            PhoneNumber = phoneNumber.Trim();
            SecondaryPhoneNumber = TrimOrNull(secondaryPhoneNumber);
            Email = TrimOrNull(email);
            Address = TrimOrNull(address);
            Notes = TrimOrNull(notes);
            IsDeleted = false;
        }

        public static Customer Create(
            string name,
            string phoneNumber,
            string? secondaryPhoneNumber = null,
            string? email = null,
            string? address = null,
            string? notes = null)
        {
            return new Customer(name, phoneNumber, secondaryPhoneNumber, email, address, notes);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be null or empty.", nameof(name));
            Name = name.Trim();
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Customer phone number cannot be null or empty.", nameof(phoneNumber));
            PhoneNumber = phoneNumber.Trim();
        }

        public void UpdateSecondaryPhoneNumber(string? secondaryPhoneNumber)
        {
            SecondaryPhoneNumber = TrimOrNull(secondaryPhoneNumber);
        }

        public void UpdateEmail(string? email)
        {
            Email = TrimOrNull(email);
        }

        public void UpdateAddress(string? address)
        {
            Address = TrimOrNull(address);
        }

        public void UpdateNotes(string? notes)
        {
            Notes = TrimOrNull(notes);
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
