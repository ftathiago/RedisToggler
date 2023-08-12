using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace RedisToggler.Lib.Test.Impl.RedisCache.RedisTypedCacheTest;

public class GetAsyncTest : RedisTypedCacheBaseTest
{
    [Fact]
    public async Task Should_ReturnAnObject_When_KeyExistsOnCacheAsync()
    {
        // Given
        var expectedObject = new SerializableObject { Property = "Object" };
        var serializedObject = JsonSerializer.Serialize(expectedObject);
        var key = Guid.NewGuid().ToString();
        DistributedCache
            .Setup(dc => dc.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UTF8Encoding.UTF8.GetBytes(serializedObject));
        var cache = BuildRedisTypedCache();

        // When
        var cachedObject = await cache
            .GetAsync(
                key: key,
                getFromSourceAsync: () => Task.FromResult<SerializableObject?>(expectedObject));

        // Then
        cachedObject.Should().NotBeNull();
        cachedObject.Should().BeEquivalentTo(expectedObject);
        cachedObject.Should().NotBe(expectedObject);
    }

    [Fact]
    public async Task Should_ReturnAnObject_When_KeyDoesNotExistsOnCacheButExistsOnSourceAsync()
    {
        // Given
        var expectedObject = new SerializableObject { Property = "Object" };
        var serializedObject = string.Empty;
        var key = Guid.NewGuid().ToString();
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        mockMethod
            .Setup(m => m())
            .ReturnsAsync(expectedObject);
        DistributedCache
            .Setup(dc => dc.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UTF8Encoding.UTF8.GetBytes(serializedObject));
        var cache = BuildRedisTypedCache();

        // When
        var cachedObject = await cache
            .GetAsync(
                key: key,
                getFromSourceAsync: mockMethod.Object);

        // Then
        cachedObject.Should().NotBeNull();
        cachedObject.Should().Be(expectedObject);
        mockMethod.Verify(m => m(), Times.Once());
    }

    [Fact]
    public async Task Should_ReturnNull_When_KeyDoesNotExistsOnCacheAndNotExistsOnSourceAsync()
    {
        // Given
        SerializableObject? expectedObject = null;
        var serializedObject = string.Empty;
        var key = Guid.NewGuid().ToString();
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        mockMethod
            .Setup(m => m())
            .ReturnsAsync(expectedObject);
        DistributedCache
            .Setup(dc => dc.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UTF8Encoding.UTF8.GetBytes(serializedObject));
        var cache = BuildRedisTypedCache();

        // When
        var cachedObject = await cache
            .GetAsync(
                key: key,
                getFromSourceAsync: mockMethod.Object);

        // Then
        cachedObject.Should().BeNull();
        mockMethod.Verify(m => m(), Times.Once());
    }

    [Fact]
    public async Task Should_NotRethrowException_When_CacheThrowsOneAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        DistributedCache
            .Setup(dc => dc.GetAsync(key, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.SocketClosed, "Something is wrong"));
        var cache = BuildRedisTypedCache();

        // When
        Func<Task> act = () => cache
            .GetAsync(
                key: key,
                getFromSourceAsync: mockMethod.Object);

        // Then
        await act.Should().NotThrowAsync<RedisException>();
        Logger.Verify(
            l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "Error while trying to retrieve data from RedisTypedCache"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        mockMethod.Verify(m => m(), Times.Never());
    }

    [Fact]
    public async Task Should_TurnOffMonitor_When_RedisGetAsyncThrowsAnExceptionAsync()
    {
        // Given
        var key = Guid.NewGuid().ToString();
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        DistributedCache
            .Setup(dc => dc.GetAsync(key, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.SocketClosed, "Something is wrong"));
        var cache = BuildRedisTypedCache();

        // When
        await cache
            .GetAsync(
                key: key,
                getFromSourceAsync: mockMethod.Object);

        // Then
        CacheMonitor.Active.Should().BeFalse();
    }
}
