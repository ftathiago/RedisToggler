using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Impl;

namespace RedisToggler.Lib;

internal interface ICacheStorageStrategy
{
    ICacheHandler Get(CacheConfig config);
}
