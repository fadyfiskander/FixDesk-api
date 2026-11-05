using FixDesk.Domain.Customers;
using FixDesk.Domain.Customers.Repositories;
using FixDesk.Infrastructure.Persistence;

namespace FixDesk.Infrastructure.Repositories.Write
{
    public class CustomerRepository : EfRepository<Customer, CustomerId>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public override Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
        {
            entity.SoftDelete();
            _context.Set<Customer>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
