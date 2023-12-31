using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Handlers;
using StackExchange.Redis;

namespace RedisToggler.Lib.Test.Impl.RedisCache.RedisCacheHandlerTest;

public class RemoveAsyncTest : RedisCacheHandlerBaseTest
{
    [Fact]
    public async Task Should_RemoveFromCache_When_KeyExistsAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cache = BuildRedisCacheHandler();

        // When
        await cache.RemoveAsync<SerializableObject>(key, CancellationToken);

        // Then
        DistributedCache.Verify(dc => dc.RemoveAsync(key.Value, CancellationToken), Times.Once());
    }

    [Fact]
    public async Task Should_NotThrowException_When_CancellationWasRequestedAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var cache = BuildRedisCacheHandler();

        // When
        Func<Task> act = () => cache.RemoveAsync<SerializableObject>(key, token);
        cancellation.Cancel();

        // Then
        await act.Should().NotThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Should_NotThrowException_When_RedisThrowsOneAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        DistributedCache
            .Setup(dc => dc.RemoveAsync(key.Value, token))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.ProtocolFailure,
                "Something went wrong"));
        var cache = BuildRedisCacheHandler();

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
                    @object.ToString() == "Error while trying to store data from RedisCacheHandler"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_TurnOffMonitor_When_RedisRemoveAsyncThrowsExceptionAsync()
    {
        // Given
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        DistributedCache
            .Setup(dc => dc.RemoveAsync(key.Value, token))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.ProtocolFailure,
                "Something went wrong"));
        var cache = BuildRedisCacheHandler();

        // When
        await cache.RemoveAsync<SerializableObject>(key, token);

        // Then
        CacheMonitor.Active.Should().BeFalse();
    }
}
