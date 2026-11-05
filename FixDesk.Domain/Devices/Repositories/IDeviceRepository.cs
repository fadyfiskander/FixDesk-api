using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Devices.Repositories
{
    // الـ Write (EF Core): إضافة/تعديل/Soft Delete (IsDeleted).
    public interface IDeviceRepository : IRepository<Device, DeviceId>
    {
    }
}
