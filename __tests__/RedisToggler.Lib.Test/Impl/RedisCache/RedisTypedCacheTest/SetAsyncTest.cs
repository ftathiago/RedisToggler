using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace RedisToggler.Lib.Test.Impl.RedisCache.RedisTypedCacheTest;

public class SetAsyncTest : RedisTypedCacheBaseTest
{
    [Fact]
    public async Task Should_StoreOnCacheWithKeyAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var serializedObject = UTF8Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(storedObject));
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cache = BuildRedisTypedCache();

        // When
        await cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        DistributedCache.Verify(
            dc => dc.SetAsync(
                key.Value,
                serializedObject,
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_NotRethrowException_When_RedisThrowsOneAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        DistributedCache
            .Setup(dc => dc.SetAsync(
                key.Value,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.InternalFailure,
                "Something went wrong"));
        var cache = BuildRedisTypedCache();

        // When
        Func<Task> act = () => cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Should_ConfigureDistributedCacheEntryOptions_When_EntryConfigurationIsSpecifiedAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cache = BuildRedisTypedCache();

        // When
        await cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        DistributedCache.Verify(
            dc => dc.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]?>(),
                It.Is<DistributedCacheEntryOptions>(opt =>
                    opt.AbsoluteExpirationRelativeToNow.Equals(EntryConfiguration.CacheDuration)
                    && opt.SlidingExpiration.Equals(EntryConfiguration.CacheSlidingDuration)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_TurnOffCacheMonitor_When_RedisSetAsyncThrowsAnExceptionAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        DistributedCache
            .Setup(dc => dc.SetAsync(
                key.Value,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.InternalFailure,
                "Something went wrong"));
        var cache = BuildRedisTypedCache();

        // When
        await cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        CacheMonitor.Active.Should().BeFalse();
    }
}
