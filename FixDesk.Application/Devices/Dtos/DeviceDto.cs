namespace FixDesk.Application.Devices.Dtos
{
    public record DeviceDto(
        Guid Id,
        string Model,
        string? SerialNumber,
        Guid CategoryId,
        string CategoryName,
        Guid BrandId,
        string BrandName,
        Guid CustomerId,
        string CustomerName,
        bool IsDeleted);
}
