using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Identity.Repositories
{
    // الـ Read (Dapper) للمستخدم.
    public interface IUserQueryRepository : IQueryRepository<User, UserId>
    {
        // الدخول بـ username — مش بيرجع المعطّلين.
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    }
}
