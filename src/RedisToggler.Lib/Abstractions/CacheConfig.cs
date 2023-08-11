namespace RedisToggler.Lib.Abstractions;

public class CacheConfig
{
    public CacheType CacheType { get; set; } = CacheType.NoCache;

    public string? ConnectionString { get; set; }
}
