using Microsoft.Extensions.Logging;
using System.Reflection;

namespace RedisToggler.Lib;

internal static class CacheMessages
{
    public const string CacheSetError = "Error while trying to store data from {source}";
    public const string CacheGetError = "Error while trying to retrieve data from {source}";
    public const string CacheRemoveError = "Error while trying to store data from {source}";

    public const string NoCacheStore =
        "The object {value} with key {key} could not be stored because log is not working. " +
        "This could make the request slowly.";

    public const string NoCacheGet =
        "The requested object, with key {key}, could not be recovered from cache, because it is " +
        "not working. This could make the request slowly.";

    public const string NoCacheRemove =
        "Could not remove object with key {key} because cache is not working.";

    public static readonly EventId NoCacheEventId = new EventId(
        id: 11082023,
        name: $"{Assembly.GetEntryAssembly()?.GetName().Name}fff67b92-465b-441f-ba80-ffee2e1dde2e");
}
