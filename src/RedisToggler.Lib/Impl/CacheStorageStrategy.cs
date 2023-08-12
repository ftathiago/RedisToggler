using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Impl.MemoryCache;
using RedisToggler.Lib.Impl.NoCache;
using RedisToggler.Lib.Impl.RedisCache;

namespace RedisToggler.Lib.Impl;

internal class CacheStorageStrategy : ICacheStorageStrategy
{
    private readonly Dictionary<CacheType, ICacheHandler> _cache = new();
    private readonly CacheMonitor _monitor;

    public CacheStorageStrategy(
        CacheMonitor monitor,
        IRedisTypedCache redisTypedCache,
        IMemoryTypedCache memoryTypedCache,
        INoCache notUseCache)
    {
        _cache.Add(CacheType.Redis, redisTypedCache);
        _cache.Add(CacheType.Memory, memoryTypedCache);
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
