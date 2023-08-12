namespace RedisToggler.Lib.Test.Impl.MemoryCacheTest;

public class SetAsyncTest : MemoryCacheBaseTest
{
    [Fact]
    public async Task Should_StoreOnMemoryCacheWithKeyAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        MemoryCache
            .Setup(mc => mc.CreateEntry(key))
            .Returns(CacheEntry.Object);
        var cache = BuildMemoryTypedCache();

        // When
        await cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        CacheEntry.VerifySet(
            ce => ce.Value = storedObject,
            Times.Once);
        CacheEntry.VerifySet(
            ce => ce.AbsoluteExpirationRelativeToNow = EntryConfiguration.CacheDuration,
            Times.Once);
        CacheEntry.VerifySet(
            ce => ce.SlidingExpiration = EntryConfiguration.CacheSlidingDuration,
            Times.Once);
    }

    [Fact]
    public async Task Should_NotRethrowException_When_MemoryThrowsOneAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        MemoryCache
            .Setup(dc => dc.CreateEntry(It.IsAny<object>()))
            .Throws(new OutOfMemoryException("Something went wrong"));
        var cache = BuildMemoryTypedCache();

        // When
        Func<Task> act = () => cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Should_ConfigureMemoryCacheEntryOptions_When_EntryConfigurationIsSpecifiedAsync()
    {
        // Given
        var storedObject = new SerializableObject { Property = "Teste" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        MemoryCache
            .Setup(mc => mc.CreateEntry(key))
            .Returns(CacheEntry.Object);
        var cache = BuildMemoryTypedCache();

        // When
        await cache.SetAsync(key, storedObject, EntryConfiguration);

        // Then
        CacheEntry.VerifySet(
            ce => ce.AbsoluteExpirationRelativeToNow = EntryConfiguration.CacheDuration,
            Times.Once);
        CacheEntry.VerifySet(
            ce => ce.SlidingExpiration = EntryConfiguration.CacheSlidingDuration,
            Times.Once);
    }
}
