
using MediatR;
using Microsoft.Extensions.Logging;
using NexoraBackend.Application.Interfaces.Behaviors;
using NexoraBackend.Application.Interfaces.Services;

namespace NexoraBackend.Application.Behaviors;
/*
If data exists in cache → return instantly
If not → execute handler → save result in cache.
*/
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
where TResponse : class
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //If query doesn’t support caching → just execute normally
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        //cache lookup
        var cached = await _cache.GetAsync<TResponse>(cacheableQuery.CacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheableQuery.CacheKey);
            return cached;
        }

        //cache miss → execute query and cache result
        var response = await next();
        await _cache.SetAsync(cacheableQuery.CacheKey, response, cacheableQuery.CacheExpiry ?? TimeSpan.FromMinutes(5), cancellationToken);
        return response;
    }
}