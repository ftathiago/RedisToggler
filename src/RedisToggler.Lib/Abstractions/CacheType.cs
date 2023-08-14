namespace RedisToggler.Lib.Abstractions;

public enum CacheType
{
    /// <summary>
    /// Cache is turned off.
    /// </summary>
    NoCache,

    /// <summary>
    /// Use Redis as cache storage.
    /// </summary>
    Redis,

    /// <summary>
    /// Use memory as cache storage.
    /// </summary>
    Memory,
}
