using MediatR;

namespace SportsData.Shared
{
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
