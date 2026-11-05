namespace FixDesk.Application.Customers.Dtos
{
    public record CustomerDto(
        Guid Id,
        string Name,
        string PhoneNumber,
        string? SecondaryPhoneNumber,
        string? Email,
        string? Address,
        string? Notes,
        bool IsDeleted);
}
