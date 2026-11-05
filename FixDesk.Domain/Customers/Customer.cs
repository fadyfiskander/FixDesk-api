using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Customers
{
    public class Customer : AggregateRoot<CustomerId>
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? SecondaryPhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        public string? Notes { get; private set; }

        [SetsRequiredMembers]
        private Customer(string firstName, string lastName, string phoneNumber, string? secondaryPhoneNumber, string email, string address, string? notes)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));

            Id = new CustomerId(Guid.NewGuid());
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            SecondaryPhoneNumber = secondaryPhoneNumber;
            Email = email;
            Address = address;
            Notes = notes;
        }

        public static Customer Create(string firstName, string lastName, string phoneNumber, string? secondaryPhoneNumber, string email, string address, string? notes)
        {
            return new Customer(firstName, lastName, phoneNumber, secondaryPhoneNumber, email, address, notes);
        }

        public void UpdateFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
            FirstName = firstName;
        }

        public void UpdateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
            LastName = lastName;
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));
            PhoneNumber = phoneNumber;
        }

        public void UpdateSecondaryPhoneNumber(string secondaryPhoneNumber)
        {
            SecondaryPhoneNumber = secondaryPhoneNumber;
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            Email = email;
        }

        public void UpdateAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));
            Address = address;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
        }
    }
}
