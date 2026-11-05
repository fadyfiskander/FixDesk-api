using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Identity.Repositories
{
    // الـ Write (EF Core): إضافة/تعديل (باسورد/display name)/تعطيل.
    public interface IUserRepository : IRepository<User, UserId>
    {
    }
}
