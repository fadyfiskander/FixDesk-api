
namespace FixDesk.Application.Common.Interfaces
{
    /// <summary>
    /// Shared unit-of-work contract.
    /// Each module exposes its own module-scoped interface
    /// inheriting this contract.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);

        int SaveChanges();

        void BeginTransaction();

        Task BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        void CommitTransaction();

        Task CommitTransactionAsync(
            CancellationToken cancellationToken = default);

        void RollbackTransaction();

        Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}
