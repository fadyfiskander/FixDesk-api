using FixDesk.Domain.Devices;
using FixDesk.Domain.Devices.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class DeviceRepository : EfRepository<Device, DeviceId>, IDeviceRepository
    {
        public DeviceRepository(AppDbContext context) : base(context)
        {
        }

        public override Task DeleteAsync(Device entity, CancellationToken cancellationToken = default)
        {
            entity.SoftDelete();
            _context.Set<Device>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
