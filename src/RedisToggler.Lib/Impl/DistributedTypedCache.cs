using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;

namespace RedisToggler.Lib.Impl;

/// <summary>
/// Instance that implements cache gathering.
/// </summary>
/// <typeparam name="TEntryConfig">What configuration should be
/// used for this cache object. Store as a Singleton.
/// <remarks>If all objects should be stored with the same configuration,
/// you can use the base CacheEntryConfiguration type.</remarks>
/// </typeparam>
public sealed class DistributedTypedCache<TEntryConfig> : IDistributedTypedCache<TEntryConfig>
    where TEntryConfig : CacheEntryConfiguration
{
    private readonly ICacheStorageStrategy _cacheStrategy;
    private readonly CacheConfig _cacheConfig;
    private readonly TEntryConfig _entryConfig;

    public DistributedTypedCache(
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
        var cacheKey = new CacheKey<TObject>(_entryConfig, key);

        return await cache.GetAsync(cacheKey, getFromSourceAsync, token);
    }

    public async Task RemoveAsync<TObject>(string key, CancellationToken token = default)
    {
        var cache = _cacheStrategy.Get(_cacheConfig);
        var cacheKey = new CacheKey<TObject>(_entryConfig, key);

        await cache.RemoveAsync<TObject>(cacheKey, token);
    }

    public async Task SetAsync<TObject>(
        string key,
        TObject value,
        CancellationToken token = default)
    {
        var cache = _cacheStrategy.Get(_cacheConfig);
        var cacheKey = new CacheKey<TObject>(_entryConfig, key);

        await cache.SetAsync(cacheKey, value, _entryConfig, token);
    }
}
