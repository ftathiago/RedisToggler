using RedisToggler.Lib.Impl;

namespace RedisToggler.Lib.Abstractions;

public interface IDistributedTypedCache<TEntryConfig>
    where TEntryConfig : CacheEntryConfiguration
{
    Task<TObject?> GetAsync<TObject>(
        string key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default);

    Task SetAsync<TObject>(
        string key,
        TObject value,
        CancellationToken token = default);

    Task RemoveAsync(
        string key,
        CancellationToken token = default);
}
