using FixDesk.Domain.Abstractions;
using FixDesk.Domain.Identity;
using System.Diagnostics.CodeAnalysis;

namespace FixDesk.Domain.Technicians
{
    // الفني صلّح الجهاز — مستقل عن User (المرجع - قسم 4). ممكن مالوش حساب.
    // الـ Owner اللي بيصلّح يُسجل Technician مربوط بحسابه (~ UserId اختياري).
    public class Technician : AggregateRoot<TechnicianId>
    {
        public string FullName { get; private set; }
        public string? PhoneNumber { get; private set; }

        // اختياري: موجود فقط لو الفني بيستخدم النظام بنفسه
        public UserId? UserId { get; private set; }

        public bool IsActive { get; private set; }

        [SetsRequiredMembers]
        private Technician(string fullName, UserId? userId, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Technician full name cannot be null or empty.", nameof(fullName));

            Id = TechnicianId.New();
            FullName = fullName.Trim();
            UserId = userId;
            PhoneNumber = TrimOrNull(phoneNumber);
            IsActive = true;
        }

        public static Technician Create(string fullName, UserId? userId = null, string? phoneNumber = null)
        {
            return new Technician(fullName, userId, phoneNumber);
        }

        public void LinkUser(UserId userId)
        {
            UserId = userId;
        }

        public void UnlinkUser()
        {
            UserId = null;
        }

        public void UpdateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Technician full name cannot be null or empty.", nameof(fullName));
            FullName = fullName.Trim();
        }

        public void UpdatePhoneNumber(string? phoneNumber)
        {
            PhoneNumber = TrimOrNull(phoneNumber);
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        private static string? TrimOrNull(string? value)
        {
            var trimmed = value?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}
