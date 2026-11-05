using Dapper;
using FixDesk.Application.Categories.Dtos;
using FixDesk.Application.Categories.Queries;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Read
{
    public class CategoryQueryRepository : QueryRepository, ICategoryQueryRepository
    {
        private const string BaseSelect =
            @"SELECT Id, Name, IsActive
              FROM " + TableCategories;

        public CategoryQueryRepository(ISqliteConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE Id = @id", new { id }, cancellationToken: cancellationToken));
            return row is null ? null : MapRow(row);
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} ORDER BY Name", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            var rows = await Connection.QueryAsync<dynamic>(new CommandDefinition(
                $"{BaseSelect} WHERE IsActive = 1 ORDER BY Name", cancellationToken: cancellationToken));
            return rows.Select(MapRow).ToList();
        }

        private static CategoryDto MapRow(dynamic row) => new(
            GuidOf(row, "Id"),
            StringOf(row, "Name"),
            BoolOf(row, "IsActive"));
    }
}
