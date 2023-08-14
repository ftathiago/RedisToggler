using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Handlers;

namespace RedisToggler.Lib.Test.Handlers.MemoryCacheHandlerTest;

public class RemoveAsyncTest : MemoryCacheHandlerBaseTest
{
    [Fact]
    public async Task Should_RemoveFromMemoryCache_When_KeyExistsAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cache = BuildMemoryCacheHandler();

        // When
        await cache.RemoveAsync(key, CancellationToken);

        // Then
        MemoryCache.Verify(mc => mc.Remove(key), Times.Once());
    }

    [Fact]
    public async Task Should_NotThrowException_When_CancellationWasRequestedToMemoryCacheAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var cache = BuildMemoryCacheHandler();

        // When
        Func<Task> act = () => cache.RemoveAsync<SerializableObject>(key, token);
        cancellation.Cancel();

        // Then
        await act.Should().NotThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Should_NotThrowException_When_MemoryThrowsOneAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        MemoryCache
            .Setup(dc => dc.Remove(key))
            .Throws(new OutOfMemoryException("Something went wrong"));
        var cache = BuildMemoryCacheHandler();

        // When
        Func<Task> act = () => cache.RemoveAsync<SerializableObject>(key, token);

        // Then
        await act.Should().NotThrowAsync();
        Logger.Verify(
            l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "Error while trying to store data from MemoryCacheHandler"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
