using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;
using RedisToggler.Lib.Handlers.MemoryCacheHandlers;
using RedisToggler.Lib.Handlers.NoCacheHandlers;
using RedisToggler.Lib.Handlers.RedisCacheHandlers;
using RedisToggler.Lib.Impl;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace RedisToggler.Lib.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Configure cache capabilities for your application.
    /// This will add Redis and InMemory caches.
    /// </summary>
    /// <param name="services">IServiceCollection instance.</param>
    /// <param name="setupCacheConfig">Configure your cache.</param>
    public static IServiceCollection AddCacheWrapper(
        this IServiceCollection services,
        Action<CacheConfig> setupCacheConfig)
    {
        var cacheConfig = new CacheConfig();

        setupCacheConfig(cacheConfig);

        services
            .Configure<RedisCacheOptions>(opt => opt.Configuration = cacheConfig.ConnectionString)
            .AddSingleton(cacheConfig)
            .AddSingleton(typeof(IDistributedTypedCache<>), typeof(DistributedTypedCache<>))
            .AddSingleton(_ => new CacheMonitor())
            .AddRedisCache()
            .AddSingleton<INoCacheHandler, NoCacheHandler>()
            .AddInMemoryCache()
            .AddSingleton<ICacheStorageStrategy, CacheStorageStrategy>();

        return services;
    }

    private static IServiceCollection AddInMemoryCache(
        this IServiceCollection services) =>
        services
            .AddMemoryCache()
            .AddSingleton<IMemoryCacheHandler, MemoryCacheHandler>();

    private static IServiceCollection AddRedisCache(
        this IServiceCollection services) =>
        services
            .AddSingleton<IRedisCacheHandler, RedisCacheHandler>()

            // Turns Connection destructive by IServiceCollection, avoiding memory leak and
            // connection "keeping open" after application shutdown.
            .AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
                var cacheMonitor = provider.GetRequiredService<CacheMonitor>();

                IConnectionMultiplexer? connection = null;
                if (options.ConnectionMultiplexerFactory != null)
                {
                    connection = options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();
                }
                else if (options.ConfigurationOptions != null)
                {
                    connection = ConnectionMultiplexer.Connect(options.ConfigurationOptions);
                }
                else if (options.Configuration != null)
                {
                    connection = ConnectionMultiplexer.Connect(options.Configuration);
                }

                if (connection is null)
                {
                    throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "All connection options are null");
                }

                connection.ConnectionFailed += (sender, args) => cacheMonitor.UpdateCache(false);
                connection.ConnectionRestored += (sender, args) => cacheMonitor.UpdateCache(true);

                return connection;
            })

            // This is need in this way, because otherwise, we will have a circular reference
            // about connection configuration. Can be removed, maybe, if reading original source,
            // we create this objects tree manually. But, we want to maintain any lib change?
            .AddSingleton<IDistributedCache>(provider => new ServiceCollection()
                .AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = null;
                    opt.ConfigurationOptions = null;
                    opt.ConnectionMultiplexerFactory = () => Task.FromResult(provider.GetRequiredService<IConnectionMultiplexer>());
                })
                .BuildServiceProvider()
                .GetRequiredService<IDistributedCache>());
}
