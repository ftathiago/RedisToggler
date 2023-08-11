namespace RedisToggler.Lib.Abstractions;

public interface IDistributedTypedCache
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
