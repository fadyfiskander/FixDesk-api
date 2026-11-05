using Dapper;
using FixDesk.Application.Technicians.Dtos;
using FixDesk.Application.Technicians.Queries;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class TechnicianQueryRepository : QueryRepository, ITechnicianQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, FullName, PhoneNumber, UserId, IsActive
              FROM " + TableTechnicians;

        public TechnicianQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<TechnicianDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<TechnicianDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} ORDER BY FullName", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        public async Task<IReadOnlyList<TechnicianDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE IsActive = 1 ORDER BY FullName", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        public async Task<TechnicianDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE UserId = @userId", new { userId }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        private static TechnicianDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "FullName"),
            NullableStringOf(row, "PhoneNumber"),
            NullableGuidOf(row, "UserId"),
            BoolOf(row, "IsActive"));
    }
}
