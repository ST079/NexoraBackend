using MediatR;

namespace NexoraBackend.Application.Behaviors;  

public class SanitizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        SanitizeStringProperties(request);
        return await next();
    }

    private static void SanitizeStringProperties(object obj)
    {
        foreach (var prop in obj.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite))
        {
            var value = (string?)prop.GetValue(obj);
            if (value != null)
                prop.SetValue(obj, value.Trim());
        }
    }
}