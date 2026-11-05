using FixDesk.Application.Identity.Dtos;

namespace FixDesk.Application.Identity.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only (never the password hash).
    public interface IUserQueryRepository
    {
        Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Login screen.
        Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    }
}
