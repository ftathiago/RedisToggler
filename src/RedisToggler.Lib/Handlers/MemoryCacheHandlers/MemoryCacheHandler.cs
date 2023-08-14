using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Configurations;

namespace RedisToggler.Lib.Handlers.MemoryCacheHandlers;

internal class MemoryCacheHandler : IMemoryCacheHandler
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheHandler> _logger;

    public MemoryCacheHandler(
        ILogger<MemoryCacheHandler> logger,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<TObject?> GetAsync<TObject>(
        CacheKey<TObject> key,
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
                args: nameof(MemoryCacheHandler),
                exception: ex);
            return default;
        }
    }

    public Task RemoveAsync<TObject>(
        CacheKey<TObject> key,
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
                args: nameof(MemoryCacheHandler),
                exception: ex);
        }

        return Task.CompletedTask;
    }

    public Task SetAsync<TObject>(
        CacheKey<TObject> key,
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
                args: nameof(MemoryCacheHandler),
                exception: ex);
        }

        return Task.CompletedTask;
    }
}
