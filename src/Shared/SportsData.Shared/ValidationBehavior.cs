using FluentValidation;
using MediatR;

namespace SportsData.Shared
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                // We need to return a Response.Failure, but TResponse is generic.
                // This usually requires a bit of reflection or a base class constraint.
                // Since our Result pattern has a static Failure method, we can use reflection or 
                // constrain `TResponse` to `Result`.
                // For simplicity here, we assume TResponse is compatible or throw for now 
                // but usually we want to return the error Result.
                
                var errors = failures.Select(f => f.ErrorMessage).ToArray();
                
                // Reflection fallback to create the correct Result type
                // This is a common pattern when mixing Generics + MediatR
                var resultType = typeof(TResponse);
                
                // Check if it's generic Result<T> or Result
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var factoryMethod = resultType.GetMethod("Failure", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (factoryMethod != null)
                    {
                        return (TResponse)factoryMethod.Invoke(null, new object[] { errors })!;
                    }
                }
                else if (resultType == typeof(Result))
                {
                     return (TResponse)(object)Result.Failure(errors);
                }
            }

            return await next();
        }
    }
}
