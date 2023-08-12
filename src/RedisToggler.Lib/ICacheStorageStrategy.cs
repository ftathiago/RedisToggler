using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Impl;

namespace RedisToggler.Lib;

public interface ICacheStorageStrategy
{
    internal ICacheHandler Get(CacheConfig config);
}
