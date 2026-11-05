using MediatR;

namespace FixDesk.Application.Common.Interfaces
{
    public interface IBaseTransactionCommand { }

    public interface ITransactionCommand : IRequest, IBaseTransactionCommand { }

    public interface ITransactionCommand<out TResponse> : IRequest<TResponse>, IBaseTransactionCommand { }

}
