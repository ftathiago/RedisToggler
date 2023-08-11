using Microsoft.Extensions.Logging;

namespace RedisToggler.Lib.Test.Impl.MemoryCacheTest;

public class RemoveAsyncTest : MemoryCacheBaseTest
{
    [Fact]
    public async Task Should_RemoveFromMemoryCache_When_KeyExistsAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cache = BuildMemoryTypedCache();

        // When
        await cache.RemoveAsync(key, CancellationToken);

        // Then
        MemoryCache.Verify(mc => mc.Remove(key), Times.Once());
    }

    [Fact]
    public async Task Should_NotThrowException_When_CancellationWasRequestedToMemoryCacheAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var cache = BuildMemoryTypedCache();

        // When
        Func<Task> act = () => cache.RemoveAsync(key, token);
        cancellation.Cancel();

        // Then
        await act.Should().NotThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Should_NotThrowException_When_MemoryThrowsOneAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        MemoryCache
            .Setup(dc => dc.Remove(key))
            .Throws(new OutOfMemoryException("Something went wrong"));
        var cache = BuildMemoryTypedCache();

        // When
        Func<Task> act = () => cache.RemoveAsync(key, token);

        // Then
        await act.Should().NotThrowAsync();
        Logger.Verify(l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "Error while trying to store data from MemoryTypedCache"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
