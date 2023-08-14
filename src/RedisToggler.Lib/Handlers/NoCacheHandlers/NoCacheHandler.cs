using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Configurations;

namespace RedisToggler.Lib.Handlers.NoCacheHandlers;

internal class NoCacheHandler : INoCacheHandler
{
    private readonly ILogger<NoCacheHandler> _logger;

    public NoCacheHandler(ILogger<NoCacheHandler> logger)
    {
        _logger = logger;
    }

    public async Task<TObject?> GetAsync<TObject>(
        CacheKey<TObject> key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default)
    {
        _logger.LogWarning(
            eventId: CacheMessages.NoCacheEventId,
            message: CacheMessages.NoCacheGet,
            args: key);

        return await getFromSourceAsync();
    }

    public Task RemoveAsync<TObject>(
        CacheKey<TObject> key,
        CancellationToken token = default)
    {
        _logger.LogWarning(
            eventId: CacheMessages.NoCacheEventId,
            message: CacheMessages.NoCacheRemove,
            key);

        return Task.CompletedTask;
    }

    public Task SetAsync<TObject>(
        CacheKey<TObject> key,
        TObject value,
        CacheEntryConfiguration entryConfiguration,
        CancellationToken token = default)
    {
        _logger.LogWarning(
            eventId: CacheMessages.NoCacheEventId,
            message: CacheMessages.NoCacheStore,
            value,
            key);
        return Task.CompletedTask;
    }
}
