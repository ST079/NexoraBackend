
using FluentValidation;

using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.API.Filters;

public class GraphQLErrorFilter : IErrorFilter
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<GraphQLErrorFilter> _logger;

    public GraphQLErrorFilter(IHostEnvironment env, ILogger<GraphQLErrorFilter> logger)
    {
        _env = env;
        _logger = logger;
    }

    public IError OnError(IError error)
    {
        if (error.Exception is DomainException domainEx)
        {
            return ErrorBuilder
                .FromError(error)
                .SetMessage(domainEx.Message)
                .Build();
        }

        if (error.Exception is ValidationException validationEx)
        {
            var messages = validationEx.Errors
                .Select(e => e.ErrorMessage)
                .ToList();

            return ErrorBuilder
                .FromError(error)
                .SetMessage(string.Join("; ", messages))
                .Build();
        }

        if (error.Exception is UnauthorizedAccessException)
        {
            return ErrorBuilder
                .FromError(error)
                .SetMessage("Unauthorized.")
                .Build();
        }

        if (error.Exception != null)
        {
            _logger.LogError(error.Exception, "Unhandled GraphQL exception");

            return _env.IsDevelopment()
                ? error
                : ErrorBuilder
                    .FromError(error)
                    .SetMessage("An internal error occurred.")
                    .Build();
        }

        return error;
    }
}