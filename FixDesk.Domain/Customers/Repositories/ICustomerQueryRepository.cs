using FixDesk.Domain.Abstractions.Repositories;

namespace FixDesk.Domain.Customers.Repositories
{
    // الـ Read (Dapper) للعميل.
    public interface ICustomerQueryRepository : IQueryRepository<Customer, CustomerId>
    {
        // البحث بالاسم أو الموبايل (شاشة فتح التذكرة: بحث بالعميل أولًا) — مش بيرجع المحذوفين.
        Task<IReadOnlyList<Customer>> SearchAsync(string term, CancellationToken cancellationToken = default);
    }
}
