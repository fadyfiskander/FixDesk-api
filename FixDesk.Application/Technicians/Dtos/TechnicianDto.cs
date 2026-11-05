namespace FixDesk.Application.Technicians.Dtos
{
    public record TechnicianDto(
        Guid Id,
        string FullName,
        string? PhoneNumber,
        Guid? UserId,
        bool IsActive);
}
