using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using RedisToggler.Lib.Extensions;
using StackExchange.Redis;

namespace RedisToggler.Lib.Test.Extensions;

public class ServiceCollectionExtensionBaseTest
{
    [Fact]
    public void Should_AddIConnectionMultiplexer_When_CallAddRedisCache()
    {
        // Given
        var service = new ServiceCollection();

        // When
        service.AddRedisCache(opt => opt.Configuration = "");

        // Then
        service.Should().Contain(descriptor => descriptor.ServiceType == typeof(IConnectionMultiplexer));
    }

    [Fact]
    public void Should_NotReconstructServiceCollection_When_RequireIDistributedCache()
    {
        // Given
        var service = new ServiceCollection()
            .AddRedisCache(opt => opt.Configuration = "");
        var provider = service.BuildServiceProvider();

        // When
        var cache = provider.GetRequiredService<IDistributedCache>();
        provider.Dispose();

        // Then
        cache.Should().NotBeNull();
    }
}
