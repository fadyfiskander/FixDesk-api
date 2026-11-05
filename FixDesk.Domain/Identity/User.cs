using FixDesk.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Identity
{
    // المستخدم بيسجّل دخول النظام (username + password hash). من غير ASP.NET Identity كامل:
    // الـ hash بيتولى PasswordHasher في الـ Infrastructure، والصلاحيات policies في الـ Application.
    public class User : AggregateRoot<UserId>
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string DisplayName { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        [SetsRequiredMembers]
        private User(string username, string passwordHash, string displayName, UserRole role, DateTime createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));

            Id = UserId.New();
            Username = username.Trim();
            PasswordHash = passwordHash;
            DisplayName = displayName.Trim();
            Role = role;
            IsActive = true;
            CreatedAtUtc = createdAtUtc;
        }

        public static User Create(string username, string passwordHash, string displayName, UserRole role, DateTime createdAtUtc)
        {
            return new User(username, passwordHash, displayName, role, createdAtUtc);
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            PasswordHash = passwordHash;
        }

        public void UpdateDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));
            DisplayName = displayName.Trim();
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
