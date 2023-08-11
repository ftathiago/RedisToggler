using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedisToggler.Lib.Abstractions;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace RedisToggler.Lib.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCacheWrapper(
        this IServiceCollection services,
        Action<CacheConfig> setupCacheConfig)
    {
        var cacheConfig = new CacheConfig();

        setupCacheConfig(cacheConfig);

        _ = cacheConfig.CacheType switch
        {
            CacheType.Redis => services.AddRedisCache(opt => opt.Configuration = cacheConfig.ConnectionString),
            _ => throw new ArgumentException("Cache config is invalid", nameof(setupCacheConfig)),
        };

        return services;
    }

    internal static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        Action<RedisCacheOptions> setupAction) =>
        services
            .Configure(setupAction)
            .AddSingleton(_ => new CacheMonitor())

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
                    throw new ArgumentNullException(nameof(options), "All connection options are null");
                }

                connection.ConnectionFailed += (sender, args) => cacheMonitor.UpdateCache(false);
                connection.ConnectionRestored += (sender, args) => cacheMonitor.UpdateCache(true);

                return connection;
            })

            // This is need in this way, because otherwise, we will have a circular reference
            // about connection configuration. Can be removed, maybe, if reading original source,
            // we create this objects tree manually. But, we want to maintain any lib change?
            .AddSingleton(provider => new ServiceCollection()
                .AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = null;
                    opt.ConfigurationOptions = null;
                    opt.ConnectionMultiplexerFactory = () => Task.FromResult(provider.GetRequiredService<IConnectionMultiplexer>());
                })
                .BuildServiceProvider()
                .GetRequiredService<IDistributedCache>());
}
