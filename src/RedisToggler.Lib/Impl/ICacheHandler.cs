namespace RedisToggler.Lib.Impl;

internal interface ICacheHandler
{
    Task<TObject?> GetAsync<TObject>(
        string key,
        Func<Task<TObject?>> getFromSourceAsync,
        CancellationToken token = default);

    Task SetAsync<TObject>(
        string key,
        TObject value,
        CacheEntryConfiguration entryConfiguration,
        CancellationToken token = default);

    Task RemoveAsync(
        string key,
        CancellationToken token = default);
}
