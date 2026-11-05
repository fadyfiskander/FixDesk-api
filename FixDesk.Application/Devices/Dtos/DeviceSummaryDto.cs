namespace FixDesk.Application.Devices.Dtos
{
    // Lightweight view for the new-ticket screen (pick device of the selected customer).
    public record DeviceSummaryDto(
        Guid Id,
        string Model,
        string? SerialNumber,
        string BrandName,
        string CategoryName,
        Guid CustomerId,
        string CustomerName);
}
