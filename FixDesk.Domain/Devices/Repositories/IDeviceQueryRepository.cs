using FixDesk.Domain.Abstractions.Repositories;
using FixDesk.Domain.Customers;

namespace FixDesk.Domain.Devices.Repositories
{
    // الـ Read (Dapper) للجهاز.
    public interface IDeviceQueryRepository : IQueryRepository<Device, DeviceId>
    {
        // أجهزة عميل معين (بتظهر وقت فتح التذكرة في نفس الشاشة) — مش بيرجع المحذوفين.
        Task<IReadOnlyList<Device>> GetAllByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    }
}
