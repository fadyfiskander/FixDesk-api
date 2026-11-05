namespace FixDesk.Domain.Abstractions.Repositories
{
    // ===== الـ Write side (يُنفذ بـ EF Core في الـ Infrastructure) =====
    // كل Aggregate Root له IRepository خاص يرثه ويضيف ما يخصه.
    // أبناء الـ Aggregate (دفعات/ملاحظات/نسخ/سجل) بتتضاف من خلال methods الـ aggregate نفسها
    // (RecordPayment / AddNote / SubmitQuote / TransitionTo) — مفيش Write repos منفصلة ليهم،
    // عشان القاعدة 8 (المرجع): دفعة + حالة + audit في transaction واحدة = SaveChanges واحدة.
    //
    // DeleteAsync = الحذف المنطقي حسب طبيعة الكيان:
    //  - Soft Delete فعلي (IsDeleted): Customer و Device
    //  - تعطيل (Deactivate): Brand و Category و User و Technician
    //  - التذاكر: الإلغاء بالحالة Cancelled (DeleteAsync ممكن ما يتنفذش أصلًا)
    public interface IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
