using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Impl;
using RedisToggler.Lib.Impl.MemoryCache;

namespace RedisToggler.Lib.Test.Impl.MemoryCacheTest;

public class MemoryCacheBaseTest : IDisposable
{
    public CancellationToken CancellationToken { get; } = CancellationToken.None;

    public Mock<IMemoryCache> MemoryCache { get; } = new Mock<IMemoryCache>(MockBehavior.Strict);

    public Mock<ICacheEntry> CacheEntry { get; } = new Mock<ICacheEntry>(MockBehavior.Loose);

    public CacheEntryConfiguration EntryConfiguration { get; } = new CacheEntryConfiguration()
    {
        Active = true,
        CacheDurationInMinutes = 5,
        CacheSlidingDurationInMinutes = 2,
    };

    internal Mock<ILogger<MemoryTypedCache>> Logger { get; } = new(MockBehavior.Loose);

    public void Dispose()
    {
        Mock.VerifyAll(MemoryCache);
    }

    internal IMemoryTypedCache BuildMemoryTypedCache() =>
      new MemoryTypedCache(
        Logger.Object,
        MemoryCache.Object);
}
