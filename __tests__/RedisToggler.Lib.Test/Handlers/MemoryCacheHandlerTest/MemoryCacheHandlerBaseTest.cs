using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers.MemoryCacheHandlers;

namespace RedisToggler.Lib.Test.Handlers.MemoryCacheHandlerTest;

public class MemoryCacheHandlerBaseTest : IDisposable
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

    internal Mock<ILogger<MemoryCacheHandler>> Logger { get; } = new(MockBehavior.Loose);

    public void Dispose()
    {
        Mock.VerifyAll(MemoryCache);
    }

    internal IMemoryCacheHandler BuildMemoryCacheHandler() =>
      new MemoryCacheHandler(
        Logger.Object,
        MemoryCache.Object);
}
