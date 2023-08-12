using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Impl;
using RedisToggler.Lib.Impl.MemoryCache;
using RedisToggler.Lib.Impl.NoCache;
using RedisToggler.Lib.Impl.RedisCache;

namespace RedisToggler.Lib.Test.Impl.CacheStorageStrategyTest;

public class CacheStorageStrategyBaseTest
{
    private readonly Mock<IRedisTypedCache> _redis = new();
    private readonly Mock<IMemoryTypedCache> _memory = new();
    private readonly Mock<INoCache> _noCache = new();
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
        strategy.Should().BeAssignableTo<IRedisTypedCache>();
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
        strategy.Should().BeAssignableTo<IMemoryTypedCache>();
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
        strategy.Should().BeAssignableTo<INoCache>();
    }

    internal ICacheStorageStrategy BuildCacheStorageStrategy() => new CacheStorageStrategy(
        _monitor,
        _redis.Object,
        _memory.Object,
        _noCache.Object);
}
