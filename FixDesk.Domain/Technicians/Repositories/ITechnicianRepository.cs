using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Technicians.Repositories
{
    // الـ Write (EF Core): إضافة/تعديل/تعطيل.
    public interface ITechnicianRepository : IRepository<Technician, TechnicianId>
    {
    }
}
