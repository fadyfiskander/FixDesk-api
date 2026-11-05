namespace FixDesk.Application.Customers.Dtos
{
    // Lightweight view for the new-ticket screen (search by name or phone).
    public record CustomerSummaryDto(
        Guid Id,
        string Name,
        string PhoneNumber,
        string? SecondaryPhoneNumber,
        string? Email,
        string? Notes);
}
