namespace RedisToggler.Lib.Configurations;

/// <summary>
/// Stores cache configuration for each object to be cached.
/// </summary>
public class CacheEntryConfiguration
{
    /// <summary>
    /// The cache for this object is active and objects should be cached.
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// How much minutes an object must be live on cache.
    /// </summary>
    public int CacheDurationInMinutes { get; set; } = 5;

    /// <summary>
    /// Set how long a cache entry could be inactive before be removed from cache.
    /// </summary>
    public int CacheSlidingDurationInMinutes { get; set; } = int.MaxValue;

    /// <summary>
    /// Define a key to be added before any cache entry key.
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// Set if cache key should store information about
    /// Thread Culture information
    /// <remarks>Default: false</remarks>
    /// </summary>
    public bool StoreLanguage { get; set; } = false;

    internal TimeSpan CacheDuration =>
        TimeSpan.FromMinutes(CacheDurationInMinutes);

    internal TimeSpan CacheSlidingDuration =>
        TimeSpan.FromMinutes(CacheSlidingDurationInMinutes);
}
