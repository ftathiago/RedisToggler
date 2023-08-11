using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisToggler.Lib.Impl.RedisCache;

internal class RedisTypedCache : IRedisTypedCache
{
    private readonly ILogger<RedisTypedCache> _logger;
    private readonly IDistributedCache _cache;

    public RedisTypedCache(
        ILogger<RedisTypedCache> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<TObject?> GetAsync<TObject>(
        string key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default)
    {
        try
        {
            var serialized = await _cache.GetStringAsync(key, token);
            if (string.IsNullOrEmpty(serialized))
            {
                return await getFromSourceAsync();
            }

            return JsonSerializer.Deserialize<TObject>(serialized);
        }
        catch (RedisException ex)
        {
            _logger.LogError(
                message: CacheMessages.CacheGetError,
                args: nameof(RedisTypedCache),
                exception: ex);
            return default;
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await _cache.RemoveAsync(key, token);
        }
        catch (RedisException ex)
        {
            _logger.LogError(
                message: CacheMessages.CacheRemoveError,
                args: nameof(RedisTypedCache),
                exception: ex);
            return;
        }
    }

    public async Task SetAsync<TObject>(
        string key,
        TObject value,
        CacheEntryConfiguration entryConfiguration,
        CancellationToken token = default)
    {
        var serialized = JsonSerializer.Serialize(value);
        try
        {
            await _cache.SetStringAsync(
                key: key,
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
            _logger.LogError(
                message: CacheMessages.CacheRemoveError,
                args: nameof(RedisTypedCache),
                exception: ex);
        }
    }
}
