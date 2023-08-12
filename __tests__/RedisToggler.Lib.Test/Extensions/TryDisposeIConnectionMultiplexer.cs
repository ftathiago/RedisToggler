using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RedisToggler.Lib.Test.Extensions;

public class TryDisposeIConnectionMultiplexer
{
    [Fact]
    public void Should_DisposeConnection_When_RequiredServiceIsScoped()
    {
        // Given
        var multiplexer = new Mock<IConnectionMultiplexer>();
        var service = new ServiceCollection();
        service.AddSingleton<IConnectionMultiplexer>(_ => multiplexer.Object);

        // When
        using (var provider = service.BuildServiceProvider())
        using (var scope = provider.CreateScope())
        {
            _ = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        }

        // Then
        multiplexer.Verify(m => m.Dispose(), Times.Once());
    }

    [Fact]
    public void Should_DisposeConnection_When_RequiredServiceIsProvider()
    {
        // Given
        var multiplexer = new Mock<IConnectionMultiplexer>();
        var service = new ServiceCollection();
        service.AddSingleton<IConnectionMultiplexer>(_ => multiplexer.Object);

        // When
        using (var provider = service.BuildServiceProvider())
        {
            _ = provider.GetRequiredService<IConnectionMultiplexer>();
        }

        // Then
        multiplexer.Verify(m => m.Dispose(), Times.Once());
    }

    [Fact]
    public void Should_Be_Empty()
    {
        // Given
        var service = new ServiceCollection();

        // When
        var count = service.Count;

        // Then
        count.Should().Be(0);
    }
}
