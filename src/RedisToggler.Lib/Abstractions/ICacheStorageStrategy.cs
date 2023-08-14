using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;

namespace RedisToggler.Lib;

public interface ICacheStorageStrategy
{
    internal ICacheHandler Get(CacheConfig config);
}
