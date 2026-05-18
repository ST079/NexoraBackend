namespace NexoraBackend.Application.Interfaces.Behaviors;
//Used cause not all queries should be cached, 
//only those that implement this interface. This way we can easily control which queries are cacheable and which are not.
public interface ICacheableQuery
{
    string CacheKey { get; } //unique key used to identify cached data, used in redis
    TimeSpan? CacheExpiry { get; } // how long cache should live
}
