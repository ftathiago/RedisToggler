using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Impl;
using RedisToggler.Lib.Impl.NoCache;

namespace RedisToggler.Lib.Test.Impl.NoCacheTest;

public class NoCacheBaseTest : IDisposable
{
    public CacheEntryConfiguration EntryConfiguration { get; } = new CacheEntryConfiguration()
    {
        Active = true,
        CacheDurationInMinutes = 5,
        CacheSlidingDurationInMinutes = 2,
    };

    public CancellationToken CancellationToken { get; } = CancellationToken.None;

    internal Mock<ILogger<NoTypedCache>> Logger { get; } = new(MockBehavior.Loose);

    public void Dispose()
    {
        Mock.VerifyAll(Logger);
    }

    [Fact]
    public async Task Should_LogSetAttempt_When_TryAddAnObjectToCacheAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cacheObject = new SerializableObject();
        var expectedMessage =
            $"The object {cacheObject} with key {key} could not be stored " +
            "because log is not working. This could make the request slowly.";
        var cache = BuildNoCache();

        // When
        await cache.SetAsync(key, cacheObject, EntryConfiguration, CancellationToken);

        // Then
        Logger.Verify(
            l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.Is<EventId>(eventId => eventId == CacheMessages.NoCacheEventId),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == expectedMessage
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_LogGetAttempt_When_TryGetAnObjectFromCacheAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cacheObject = new SerializableObject();
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        mockMethod
            .Setup(m => m())
            .ReturnsAsync(cacheObject);
        var expectedMessage =
            $"The requested object, with key {key}, could not be recovered from cache, " +
            "because it is not working. This could make the request slowly.";
        var cache = BuildNoCache();

        // When
        await cache.GetAsync(key, mockMethod.Object, CancellationToken);

        // Then
        Logger.Verify(
            l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.Is<EventId>(eventId => eventId == CacheMessages.NoCacheEventId),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == expectedMessage
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_LogRemoveAttempt_When_TryRemoveAnObjectOnCacheAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var expectedMessage =
            $"Could not remove object with key {key} because cache is not working.";
        var cache = BuildNoCache();

        // When
        await cache.RemoveAsync<SerializableObject>(key, CancellationToken);

        // Then
        Logger.Verify(
            l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.Is<EventId>(eventId => eventId == CacheMessages.NoCacheEventId),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == expectedMessage
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    internal INoCache BuildNoCache() => new NoTypedCache(
        Logger.Object);
}
