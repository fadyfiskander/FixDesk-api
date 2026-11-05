using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Customers.Repositories
{
    // الـ Write (EF Core): إضافة/تعديل/Soft Delete (IsDeleted).
    public interface ICustomerRepository : IRepository<Customer, CustomerId>
    {
    }
}
