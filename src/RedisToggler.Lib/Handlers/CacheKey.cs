using RedisToggler.Lib.Configurations;
using System.Text;

namespace RedisToggler.Lib.Handlers;

internal readonly struct CacheKey<TObject>
{
    private readonly CacheEntryConfiguration _cacheConfiguration;
    private readonly string _key;

    public CacheKey(
        CacheEntryConfiguration cacheConfiguration,
        string key)
    {
        _cacheConfiguration = cacheConfiguration;
        _key = key;
    }

    /// <summary>
    /// Formated cache key
    /// </summary>
    /// <returns>Returns Cache key as string.</returns>
    public string Value =>
        $"{GetPrefix()}{GetTypeDescription()}{GetThreadCulture()}{_key}";

    private string GetPrefix() => string.IsNullOrEmpty(_cacheConfiguration.KeyPrefix)
        ? string.Empty
        : $"{_cacheConfiguration.KeyPrefix}:";

    private string GetTypeDescription() =>
        $"{GetTypeName(typeof(TObject))}:";

    private string GetThreadCulture() => _cacheConfiguration.StoreLanguage
            ? $"{Thread.CurrentThread.CurrentCulture.Name}:"
            : string.Empty;

    private StringBuilder GetTypeName(Type type, StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        sb
            .Append(type.Namespace)
            .Append('.');

        if (type.IsGenericType)
        {
            return FormatGenericTypes(sb, type);
        }

        return sb.Append(type.Name);
    }

    // When is a generic type, the `type.Name` returns something like
    // "IEnumerable`1". The code bellow remove, turning into a more friendly text.
    private StringBuilder FormatGenericTypes(StringBuilder sb, Type type)
    {
        var name = type.Name[..type.Name.LastIndexOf('`')];

        sb
            .Append(name)
            .Append('<');
        foreach (var arg in type.GenericTypeArguments)
        {
            GetTypeName(arg, sb)
                .Append(", ");
        }

        sb
            .Remove(sb.Length - 2, 2)
            .Append('>');

        return sb;
    }
}
