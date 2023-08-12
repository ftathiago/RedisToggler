using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RedisToggler.Lib.Test.Impl.RedisCache.RedisTypedCacheTest;

public class RemoveAsyncTest : RedisTypedCacheBaseTest
{
    [Fact]
    public async Task Should_RemoveFromCache_When_KeyExistsAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cache = BuildRedisTypedCache();

        // When
        await cache.RemoveAsync(key, CancellationToken);

        // Then
        DistributedCache.Verify(dc => dc.RemoveAsync(key, CancellationToken), Times.Once());
    }

    [Fact]
    public async Task Should_NotThrowException_When_CancellationWasRequestedAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var cache = BuildRedisTypedCache();

        // When
        Func<Task> act = () => cache.RemoveAsync(key, token);
        cancellation.Cancel();

        // Then
        await act.Should().NotThrowAsync<OperationCanceledException>();
    }



    [Fact]
    public async Task Should_NotThrowException_When_RedisThrowsOneAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        DistributedCache
            .Setup(dc => dc.RemoveAsync(key, token))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.ProtocolFailure,
                "Something went wrong"));
        var cache = BuildRedisTypedCache();

        // When
        Func<Task> act = () => cache.RemoveAsync(key, token);

        // Then
        await act.Should().NotThrowAsync();
        Logger.Verify(l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "Error while trying to store data from RedisTypedCache"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_TurnOffMonitor_When_RedisRemoveAsyncThrowsExceptionAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        DistributedCache
            .Setup(dc => dc.RemoveAsync(key, token))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.ProtocolFailure,
                "Something went wrong"));
        var cache = BuildRedisTypedCache();

        // When
        await cache.RemoveAsync(key, token);

        // Then
        CacheMonitor.Active.Should().BeFalse();
    }
}
