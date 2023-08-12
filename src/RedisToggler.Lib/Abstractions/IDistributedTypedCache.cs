using RedisToggler.Lib.Impl;

namespace RedisToggler.Lib.Abstractions;

#pragma warning disable S2326
/// <summary>
/// Interface for object recovering from cache.
/// </summary>
/// <typeparam name="TEntryConfig">What configuration should be
/// used for this cache object. Store as a Singleton.
/// <remarks>If all objects should be stored with the same configuration,
/// you can use the base CacheEntryConfiguration type.</remarks>
/// </typeparam>
public interface IDistributedTypedCache<TEntryConfig>
    where TEntryConfig : CacheEntryConfiguration
{
    /// <summary>
    /// Retrieve a object from cache, based on his informed
    /// key.
    /// </summary>
    /// <typeparam name="TObject">The object type stored in cache.</typeparam>
    /// <param name="key">The key for searching.</param>
    /// <param name="getFromSourceAsync">A method that retrieve object from
    /// his original source.</param>
    /// <param name="token">Cancellation Token</param>
    Task<TObject?> GetAsync<TObject>(
        string key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default);

    /// <summary>
    /// Store a object into cache.
    /// </summary>
    /// <typeparam name="TObject">The object type stored in cache.</typeparam>
    /// <param name="key">The key for searching.</param>
    /// <param name="value">The object instance.</param>
    /// <param name="token">Cancellation Token</param>
    Task SetAsync<TObject>(
        string key,
        TObject value,
        CancellationToken token = default);

    /// <summary>
    /// Remove a entry from cache.
    /// </summary>
    /// <param name="key">The key for searching.</param>
    /// <param name="token">Cancellation Token</param>
    Task RemoveAsync(
        string key,
        CancellationToken token = default);
}
#pragma warning restore S2326
