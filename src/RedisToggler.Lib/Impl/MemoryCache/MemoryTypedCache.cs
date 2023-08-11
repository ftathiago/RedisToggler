using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace RedisToggler.Lib.Impl.MemoryCache;

internal class MemoryTypedCache : IMemoryTypedCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryTypedCache> _logger;

    public MemoryTypedCache(
        ILogger<MemoryTypedCache> logger,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<TObject?> GetAsync<TObject>(
        string key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default)
    {
        try
        {
            var entry = _memoryCache.TryGetValue(key, out TObject? result);
            if (entry)
            {
                return result;
            }

            return await getFromSourceAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                message: CacheMessages.CacheGetError,
                args: nameof(MemoryTypedCache),
                exception: ex);
            return default;
        }
    }

    public Task RemoveAsync(
        string key,
        CancellationToken token = default)
    {
        try
        {
            _memoryCache.Remove(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                message: CacheMessages.CacheRemoveError,
                args: nameof(MemoryTypedCache),
                exception: ex);
        }

        return Task.CompletedTask;
    }

    public Task SetAsync<TObject>(
        string key,
        TObject value,
        CacheEntryConfiguration entryConfiguration,
        CancellationToken token = default)
    {
        try
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = entryConfiguration.CacheDuration,
                SlidingExpiration = entryConfiguration.CacheSlidingDuration,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                message: CacheMessages.CacheRemoveError,
                args: nameof(MemoryTypedCache),
                exception: ex);
        }

        return Task.CompletedTask;
    }
}
