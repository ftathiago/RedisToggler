using Microsoft.Extensions.Logging;
using RedisToggler.Lib.Handlers;

namespace RedisToggler.Lib.Test.Handlers.MemoryCacheHandlerTest;

public class GetAsyncTest : MemoryCacheHandlerBaseTest
{
    [Fact]
    public async Task Should_ReturnAnObject_When_KeyExistsOnCacheAsync()
    {
        // Given
        object expectedObject = new SerializableObject { Property = "Object" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        MemoryCache
            .Setup(mc => mc.TryGetValue(key, out expectedObject))
            .Returns(true);
        var cache = BuildMemoryCacheHandler();

        // When
        var cachedObject = await cache
            .GetAsync(
                key: key,
                getFromSourceAsync: () => Task.FromResult<SerializableObject?>(expectedObject as SerializableObject));

        // Then
        cachedObject.Should().NotBeNull();
        cachedObject.Should().BeEquivalentTo(expectedObject);
    }

    [Fact]
    public async Task Should_ReturnAnObject_When_KeyDoesNotExistsOnCacheButExistsOnSourceAsync()
    {
        // Given
        object? nullObjectCache = null;
        var expectedObject = new SerializableObject { Property = "Object" };
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        mockMethod
            .Setup(m => m())
            .ReturnsAsync(expectedObject);
        MemoryCache
            .Setup(mc => mc.TryGetValue(key, out nullObjectCache))
            .Returns(false);
        var cache = BuildMemoryCacheHandler();

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
        object? expectedObject = null;
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        mockMethod
            .Setup(m => m())
            .ReturnsAsync((SerializableObject?)expectedObject);
        MemoryCache
            .Setup(mc => mc.TryGetValue(key, out expectedObject))
            .Returns(false);
        var cache = BuildMemoryCacheHandler();

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
        object? expectedObject = null;
        var key = new CacheKey<SerializableObject>(EntryConfiguration, Guid.NewGuid().ToString());
        var mockMethod = new Mock<Func<Task<SerializableObject?>>>(MockBehavior.Strict);
        MemoryCache
            .Setup(mc => mc.TryGetValue(key, out expectedObject))
            .Throws(new Exception("Something is wrong"));
        var cache = BuildMemoryCacheHandler();

        // When
        Func<Task> act = () => cache
            .GetAsync(
                key: key,
                getFromSourceAsync: mockMethod.Object);

        // Then
        await act.Should().NotThrowAsync<Exception>();
        Logger.Verify(
            l =>
            l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "Error while trying to retrieve data from MemoryCacheHandler"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        mockMethod.Verify(m => m(), Times.Never());
    }
}
