using RedisToggler.Lib.Configurations;

namespace RedisToggler.Lib.Handlers;

internal interface ICacheHandler
{
    Task<TObject?> GetAsync<TObject>(
        CacheKey<TObject> key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default);

    Task SetAsync<TObject>(
        CacheKey<TObject> key,
        TObject value,
        CacheEntryConfiguration entryConfiguration,
        CancellationToken token = default);

    Task RemoveAsync<TObject>(
        CacheKey<TObject> key,
        CancellationToken token = default);
}
