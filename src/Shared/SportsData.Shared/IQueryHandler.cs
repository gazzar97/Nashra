using MediatR;

namespace SportsData.Shared
{ 
    
  /// <summary>
  /// Handler interface for queries.
  /// </summary>
  /// <typeparam name="TQuery">Query type</typeparam>
  /// <typeparam name="TResponse">Response type</typeparam>
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
