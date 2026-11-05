namespace FixDesk.Domain.Abstractions.Repositories
{
    // ===== الـ Read side (يُنفذ بـ Dapper في الـ Infrastructure) =====
    // العامل المشترك بين كل الكيانات: Get بالـ Id و GetAll.
    // كل Model له IQueryRepository خاص يرثه ويضيف ما يخصه.
    // ملاحظة: GetAllAsync مش بيرجع المحذوفين منطقيًا (Soft Delete) — الفلترة في الـ Infrastructure.
    public interface IQueryRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
