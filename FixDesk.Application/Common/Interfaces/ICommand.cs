using MediatR;

namespace FixDesk.Application.Common.Interfaces
{
    // 1. واجهة علامة فارغة تماماً (Marker)
    public interface IBaseCommand { }

    // 2. الواجهة التي لا ترجع قيمة
    public interface ICommand : IRequest, IBaseCommand { }

    // 3. الواجهة التي ترجع قيمة
    public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand { }
}
