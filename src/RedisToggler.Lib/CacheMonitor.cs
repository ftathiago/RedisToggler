namespace RedisToggler.Lib;

/// <summary>
/// Store the Redis Status (on/off) to toggle cache capability.
/// </summary>
internal class CacheMonitor
{
    private readonly object _lock = new();

    private volatile bool _active = true;

    public bool Active => _active;


    /// <summary>
    /// Update cache status, setting according with `active` parameter.
    /// </summary>
    /// <param name="active">Inform if redis is active (true) or not (false).</param>
    public void UpdateCache(bool active)
    {
        lock (_lock)
        {
            _active = active;
        }
    }
}
