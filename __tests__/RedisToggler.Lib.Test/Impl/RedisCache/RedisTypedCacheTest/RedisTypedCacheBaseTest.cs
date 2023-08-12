using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Impl;
using RedisToggler.Lib.Impl.RedisCache;

namespace RedisToggler.Lib.Test.Impl.RedisCache.RedisTypedCacheTest;

public abstract class RedisTypedCacheBaseTest : IDisposable
{
    public RedisTypedCacheBaseTest()
    {
        CacheMonitor.UpdateCache(true);
    }

    public CacheEntryConfiguration EntryConfiguration { get; } = new CacheEntryConfiguration()
    {
        Active = true,
        CacheDurationInMinutes = 5,
        CacheSlidingDurationInMinutes = 2,
    };

    public CancellationToken CancellationToken { get; } = new CancellationToken();

    public Mock<IDistributedCache> DistributedCache { get; } = new Mock<IDistributedCache>();

    internal CacheMonitor CacheMonitor { get; } = new CacheMonitor();

    internal Mock<ILogger<RedisTypedCache>> Logger { get; } = new Mock<ILogger<RedisTypedCache>>();

    public void Dispose()
    {
        Mock.VerifyAll(DistributedCache);
    }

    internal ICacheHandler BuildRedisTypedCache() =>
        new RedisTypedCache(
            Logger.Object,
            DistributedCache.Object,
            CacheMonitor);
}
