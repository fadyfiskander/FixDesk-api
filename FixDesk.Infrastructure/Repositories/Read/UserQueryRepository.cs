using Dapper;
using FixDesk.Application.Identity.Dtos;
using FixDesk.Application.Identity.Queries;
using FixDesk.Domain.Identity;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class UserQueryRepository : QueryRepository, IUserQueryRepository
    {
        // PasswordHash is never selected: authentication data must not leave persistence.
        private const string BaseSelect =
            @"SELECT Id, Username, DisplayName, Role, IsActive, CreatedAtUtc
              FROM " + TableUsers;

        public UserQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} ORDER BY DisplayName", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        public async Task<UserDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Username = @username", new { username = username.Trim() }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        private static UserDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "Username"),
            StringOf(row, "DisplayName"),
            EnumOf<UserRole>(row, "Role"),
            BoolOf(row, "IsActive"),
            DateTimeUtcOf(row, "CreatedAtUtc"));
    }
}
