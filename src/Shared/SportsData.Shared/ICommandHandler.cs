using MediatR;

namespace SportsData.Shared
{
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
