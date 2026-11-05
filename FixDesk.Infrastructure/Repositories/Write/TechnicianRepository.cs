using FixDesk.Domain.Technicians;
using FixDesk.Domain.Technicians.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class TechnicianRepository : EfRepository<Technician, TechnicianId>, ITechnicianRepository
    {
        public TechnicianRepository(AppDbContext context) : base(context)
        {
        }

        public override Task DeleteAsync(Technician entity, CancellationToken cancellationToken = default)
        {
            entity.Deactivate();
            _context.Set<Technician>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
