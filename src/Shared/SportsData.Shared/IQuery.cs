using MediatR;

namespace SportsData.Shared
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
