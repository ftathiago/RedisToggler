using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Configurations;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisToggler.Lib.Handlers.RedisCacheHandlers;

internal class RedisCacheHandler : IRedisCacheHandler
{
    private readonly ILogger<RedisCacheHandler> _logger;
    private readonly IDistributedCache _cache;
    private readonly CacheMonitor _monitor;

    public RedisCacheHandler(
        ILogger<RedisCacheHandler> logger,
        IDistributedCache cache,
        CacheMonitor monitor)
    {
        _logger = logger;
        _cache = cache;
        _monitor = monitor;
    }

    public async Task<TObject?> GetAsync<TObject>(
        CacheKey<TObject> key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default)
    {
        try
        {
            var serialized = await _cache.GetStringAsync(key.Value, token);
            if (string.IsNullOrEmpty(serialized))
            {
                return await getFromSourceAsync();
            }

            return JsonSerializer.Deserialize<TObject>(serialized);
        }
        catch (RedisException ex)
        {
            _monitor.UpdateCache(false);
            _logger.LogError(
                exception: ex,
                message: CacheMessages.CacheGetError,
                args: nameof(RedisCacheHandler));
            return default;
        }
    }

    public async Task RemoveAsync<TObject>(
        CacheKey<TObject> key,
        CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await _cache.RemoveAsync(key.Value, token);
        }
        catch (RedisException ex)
        {
            _monitor.UpdateCache(false);
            _logger.LogError(
                exception: ex,
                message: CacheMessages.CacheRemoveError,
                args: nameof(RedisCacheHandler));
        }
    }

    public async Task SetAsync<TObject>(
        CacheKey<TObject> key,
        TObject value,
        CacheEntryConfiguration entryConfiguration,
        CancellationToken token = default)
    {
        var serialized = JsonSerializer.Serialize(value);
        try
        {
            await _cache.SetStringAsync(
                key: key.Value,
                value: serialized,
                options: new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = entryConfiguration.CacheDuration,
                    SlidingExpiration = entryConfiguration.CacheSlidingDuration,
                },
                token);
        }
        catch (RedisException ex)
        {
            _monitor.UpdateCache(false);
            _logger.LogError(
                exception: ex,
                message: CacheMessages.CacheRemoveError,
                args: nameof(RedisCacheHandler));
        }
    }
}
