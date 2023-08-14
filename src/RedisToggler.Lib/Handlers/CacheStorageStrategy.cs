using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers.MemoryCacheHandlers;
using RedisToggler.Lib.Handlers.NoCacheHandlers;
using RedisToggler.Lib.Handlers.RedisCacheHandlers;

namespace RedisToggler.Lib.Handlers;

internal class CacheStorageStrategy : ICacheStorageStrategy
{
    private readonly Dictionary<CacheType, ICacheHandler> _cache = new();
    private readonly CacheMonitor _monitor;

    public CacheStorageStrategy(
        CacheMonitor monitor,
        IRedisCacheHandler redisCacheHandler,
        IMemoryCacheHandler memoryCacheHandler,
        INoCacheHandler notUseCache)
    {
        _cache.Add(CacheType.Redis, redisCacheHandler);
        _cache.Add(CacheType.Memory, memoryCacheHandler);
        _cache.Add(CacheType.NoCache, notUseCache);
        _monitor = monitor;
    }

    public ICacheHandler Get(CacheConfig config)
    {
        if (_monitor.Active)
        {
            return _cache[config.CacheType];
        }

        return _cache[CacheType.NoCache];
    }
}
