using FixDesk.Domain.Identity;

namespace FixDesk.Application.Identity.Dtos
{
    // No PasswordHash here: authentication data must never leak to the UI.
    public record UserDto(
        Guid Id,
        string Username,
        string DisplayName,
        UserRole Role,
        bool IsActive,
        DateTime CreatedAtUtc);
}
