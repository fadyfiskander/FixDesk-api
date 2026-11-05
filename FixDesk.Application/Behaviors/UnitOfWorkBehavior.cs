using Inventory.Application.Common.Interfaces;
using MediatR;

namespace FixDesk.Application.Behaviors;

public class UnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IInventoryUnitOfWork _unitOfWork;

    public UnitOfWorkBehavior(IInventoryUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Commands only (IBaseCommand) reach this behavior. Save AFTER the handler so all
        // tracked changes (including aggregate mutations inside the handler) commit together.
        var response = await next();

        // Note: domain events dispatcher runs inside InventoryDbContext.SaveChangesAsync
        // (after the base save) so handlers can observe persisted state.
        // RowVersion conflicts are mapped to InvalidOperationException by the UnitOfWork
        // (Infrastructure) so callers get a clear application-level retry message.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
