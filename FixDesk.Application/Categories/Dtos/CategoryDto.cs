namespace FixDesk.Application.Categories.Dtos
{
    public record CategoryDto(
        Guid Id,
        string Name,
        bool IsActive);
}
