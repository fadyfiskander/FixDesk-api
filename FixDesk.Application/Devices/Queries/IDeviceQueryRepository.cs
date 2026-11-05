using FixDesk.Application.Devices.Dtos;

namespace FixDesk.Application.Devices.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface IDeviceQueryRepository
    {
        Task<DeviceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Logical-delete rule: deleted devices never leave the screen lists.
        Task<IReadOnlyList<DeviceDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Devices of one customer (new-ticket screen).
        Task<IReadOnlyList<DeviceSummaryDto>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}
