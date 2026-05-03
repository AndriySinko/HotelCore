// MediatR pipeline behavior that runs FluentValidation before every command or query handler
// if any validator fails the request is rejected with a ValidationException before it reaches the handler
// this means we never have to manually call validator.Validate() inside a handler
using FluentValidation;
using MediatR;

namespace HotelCore.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // skip validation if no validators are registered for this request type
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // run all validators in parallel for better performance
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // collect all errors from all validators
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        // the GlobalExceptionHandler catches this and returns a 400 with the validation messages
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
