using FixDesk.Domain.Identity;
using FixDesk.Domain.Identity.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class UserRepository : EfRepository<User, UserId>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public override Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            entity.Deactivate();
            _context.Set<User>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
