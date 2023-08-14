using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;
using RedisToggler.Lib.Handlers.RedisCacheHandlers;

namespace RedisToggler.Lib.Test.Impl.RedisCache.RedisCacheHandlerTest;

public abstract class RedisCacheHandlerBaseTest : IDisposable
{
    protected RedisCacheHandlerBaseTest()
    {
        CacheMonitor.UpdateCache(true);
    }

    public CacheEntryConfiguration EntryConfiguration { get; } = new CacheEntryConfiguration()
    {
        Active = true,
        CacheDurationInMinutes = 5,
        CacheSlidingDurationInMinutes = 2,
    };

    public CancellationToken CancellationToken { get; } = CancellationToken.None;

    public Mock<IDistributedCache> DistributedCache { get; } = new Mock<IDistributedCache>();

    internal CacheMonitor CacheMonitor { get; } = new CacheMonitor();

    internal Mock<ILogger<RedisCacheHandler>> Logger { get; } = new Mock<ILogger<RedisCacheHandler>>();

    public void Dispose()
    {
        Mock.VerifyAll(DistributedCache);
    }

    internal ICacheHandler BuildRedisCacheHandler() =>
        new RedisCacheHandler(
            Logger.Object,
            DistributedCache.Object,
            CacheMonitor);
}
