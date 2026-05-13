using HotChocolate.Execution;
using NexoraBackend.Common.Exceptions;

namespace NexoraBackend.API.Filters;

public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is CustomException ex)
        {
            return ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetCode(ex.StatusCode.ToString())
                .Build();
        }

        if (error.Code == "AUTH_NOT_AUTHORIZED" || error.Message == "The current user is not authorized to access this resource.")
        {
            return error.WithMessage("You do not have permission to perform this action.")
                        .WithCode("403");
        }

        return ErrorBuilder.New()
            .SetMessage(error.Message)
            .Build();
    }
}