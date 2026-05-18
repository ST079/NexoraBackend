namespace NexoraBackend.Application.Behaviors;

using FluentValidation;
using MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
    //TRequest must be a MediatR request that returns TResponse
{
    // ValidationBehavior<TRequest, TResponse> : means work for any request and response type.
    // IPipelineBehavior<TRequest, TResponse> : is an interface from MediatR that allows us to define a behavior that will be executed before and after the request handler is executed.

    private readonly IEnumerable<IValidator<TRequest>> _validators; //validator injection

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //if there are no validators, just continue to the next behavior or handler.
        if (!_validators.Any()) return await next();

        //create a validation context for the request.
        var context = new ValidationContext<TRequest>(request);

        //validate the request with all validators.
        /*
        If you have 3 validators:
        runs all 3 at same time (async parallel), faster performance
        */
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        //combine all validation errors into one list
        var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        //Example result:
        //Email is required
        //Password too short
        //Name required

        if (failures.Count != 0)
        {
            //if there are validation errors, throw an exception with the errors.
            throw new ValidationException(failures);
        }

        return await next();
    }
}