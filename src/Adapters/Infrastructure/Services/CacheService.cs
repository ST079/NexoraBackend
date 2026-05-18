
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Config;
using StackExchange.Redis;
using System.Text.Json;

namespace NexoraBackend.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly RedisSettings _settings;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IConnectionMultiplexer redis, IOptions<RedisSettings> options,
        ILogger<CacheService> logger)
    {
        _db = redis.GetDatabase();
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache get failed for key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value);
            var ttl = expiry ?? TimeSpan.FromMinutes(_settings.DefaultExpiryMinutes);
            await _db.StringSetAsync(key, serialized, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache set failed for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try { await _db.KeyDeleteAsync(key); }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache remove failed for key {Key}", key); }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        try
        {
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Any())
                await _db.KeyDeleteAsync(keys);
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Cache prefix remove failed for prefix {Prefix}", prefix); }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory,
        TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        var cached = await GetAsync<T>(key, ct);
        if (cached is not null) return cached;

        var value = await factory();
        await SetAsync(key, value, expiry, ct);
        return value;
    }
}