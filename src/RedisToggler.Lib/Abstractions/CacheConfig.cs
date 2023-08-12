namespace RedisToggler.Lib.Abstractions;

/// <summary>
/// Cache configurations.
/// </summary>
public class CacheConfig
{
    /// <summary>
    /// Specify what kind of cache will be used.
    /// <remark>Default: CacheType.NoCache</remark>
    /// </summary>
    public CacheType CacheType { get; set; } = CacheType.NoCache;

    /// <summary>
    /// Inform the Redis Connection string.
    /// </summary>
    public string? ConnectionString { get; set; }
}
