using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;
using RedisToggler.Lib.Handlers.MemoryCacheHandlers;
using RedisToggler.Lib.Handlers.NoCacheHandlers;
using RedisToggler.Lib.Handlers.RedisCacheHandlers;

namespace RedisToggler.Lib.Test.CacheStorageStrategyTest;

public class CacheStorageStrategyBaseTest
{
    private readonly Mock<IRedisCacheHandler> _redis = new();
    private readonly Mock<IMemoryCacheHandler> _memory = new();
    private readonly Mock<INoCacheHandler> _noCache = new();
    private readonly CacheMonitor _monitor = new();

    [Fact]
    public void Should_ReturnRedisHandler_When_ConfigurationRequiresRedis()
    {
        // Given
        var config = new CacheConfig { CacheType = CacheType.Redis };
        var storageStrategy = BuildCacheStorageStrategy();

        // When
        var strategy = storageStrategy.Get(config);

        // Then
        strategy.Should().BeAssignableTo<IRedisCacheHandler>();
    }

    [Fact]
    public void Should_ReturnMemoryHandler_When_ConfigurationRequiresMemory()
    {
        // Given
        var config = new CacheConfig { CacheType = CacheType.Memory };
        var storageStrategy = BuildCacheStorageStrategy();

        // When
        var strategy = storageStrategy.Get(config);

        // Then
        strategy.Should().BeAssignableTo<IMemoryCacheHandler>();
    }

    [Fact]
    public void Should_ReturnNoCacheHandler_When_ConfigurationRequiresNoCache()
    {
        // Given
        var config = new CacheConfig { CacheType = CacheType.NoCache };
        var storageStrategy = BuildCacheStorageStrategy();

        // When
        var strategy = storageStrategy.Get(config);

        // Then
        strategy.Should().BeAssignableTo<INoCacheHandler>();
    }

    [Fact]
    public void Should_AlwaysReturnNoCache_When_MonitorIsNotActive()
    {
        // Given
        _monitor.UpdateCache(false);
        var config = new CacheConfig { CacheType = CacheType.Redis };
        var storageStrategy = BuildCacheStorageStrategy();

        // When
        var strategy = storageStrategy.Get(config);

        // Then
        strategy.Should().BeAssignableTo<INoCacheHandler>();
    }

    internal ICacheStorageStrategy BuildCacheStorageStrategy() => new CacheStorageStrategy(
        _monitor,
        _redis.Object,
        _memory.Object,
        _noCache.Object);
}
