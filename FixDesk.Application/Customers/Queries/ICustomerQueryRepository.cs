using FixDesk.Application.Customers.Dtos;

namespace FixDesk.Application.Customers.Queries
{
    // Read contract (executed by Dapper in Infrastructure). Returns DTOs only.
    public interface ICustomerQueryRepository
    {
        Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Logical-delete rule: deleted customers never leave the screen lists.
        Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Search by name or phone (new-ticket screen).
        Task<IReadOnlyList<CustomerSummaryDto>> SearchAsync(string term, CancellationToken cancellationToken = default);
    }
}
