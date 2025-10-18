using FastVocab.Shared.Utils;
using FluentValidation;
using MediatR;

namespace FastVocab.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests and returns Result instead of throwing exceptions
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
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

        if (failures.Any())
        {
            // Check if TResponse is Result<T> or Result
            var responseType = typeof(TResponse);
            
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Create Result<T>.Failure with validation errors
                var dataType = responseType.GetGenericArguments()[0];
                var failureMethod = responseType.GetMethod("Failure", new[] { typeof(List<Error>) });
                var validationErrors = Error.ValidationErrors(failures.Select(f => (f.PropertyName, f.ErrorMessage)));
                
                return (TResponse)failureMethod!.Invoke(null, new object[] { validationErrors })!;
            }
            else if (responseType == typeof(Result))
            {
                // Create Result.Failure with validation errors
                var validationErrors = Error.ValidationErrors(failures.Select(f => (f.PropertyName, f.ErrorMessage)));
                return (TResponse)(object)Result.Failure(validationErrors);
            }
            else
            {
                // Fallback to throwing exception for non-Result types
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}

