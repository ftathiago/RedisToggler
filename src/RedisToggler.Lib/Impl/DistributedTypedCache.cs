using RedisToggler.Lib.Abstractions;

namespace RedisToggler.Lib.Impl;

public sealed class DistributedTypedCache<TEntryConfig> : IDistributedTypedCache
    where TEntryConfig : CacheEntryConfiguration
{
    private readonly ICacheStorageStrategy _cacheStrategy;
    private readonly CacheConfig _cacheConfig;
    private readonly TEntryConfig _entryConfig;

    internal DistributedTypedCache(
        ICacheStorageStrategy cacheStrategy,
        CacheConfig cacheConfig,
        TEntryConfig entryConfig)
    {
        _cacheStrategy = cacheStrategy;
        _cacheConfig = cacheConfig;
        _entryConfig = entryConfig;
    }

    public async Task<TObject?> GetAsync<TObject>(string key, Func<Task<TObject?>> getFromSourceAsync, CancellationToken token = default)
    {
        var cache = _cacheStrategy.Get(_cacheConfig);
        return await cache.GetAsync(key, getFromSourceAsync, token);
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        var cache = _cacheStrategy.Get(_cacheConfig);
        await cache.RemoveAsync(key, token);
    }

    public async Task SetAsync<TObject>(
        string key,
        TObject value,
        CancellationToken token = default)
    {
        var cache = _cacheStrategy.Get(_cacheConfig);

        await cache.SetAsync(key, value, _entryConfig, token);
    }
}
