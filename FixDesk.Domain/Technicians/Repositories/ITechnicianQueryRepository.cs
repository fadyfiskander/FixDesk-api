using FixDesk.Domain.Abstractions.Repositories;
using FixDesk.Domain.Identity;

namespace FixDesk.Domain.Technicians.Repositories
{
    // الـ Read (Dapper) للفني.
    public interface ITechnicianQueryRepository : IQueryRepository<Technician, TechnicianId>
    {
        // الفنيين الفعالين للاختيار في التذكرة
        Task<IReadOnlyList<Technician>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        // الفني المربوط بحساب معيّن (الـ Owner اللي بيصلّح)
        Task<Technician?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    }
}
