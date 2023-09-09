using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RedisToggler.Api;

[ExcludeFromCodeCoverage]
public sealed class SupportedCultures : ReadOnlyCollection<CultureInfo>
{
    public static readonly SupportedCultures Default = new(CultureInfo.CurrentCulture);

    public SupportedCultures(CultureInfo parent)
        : base(new[] { parent })
    {
        Parent = parent;
    }

    public SupportedCultures(
        CultureInfo parent,
        IList<CultureInfo> list)
        : base(list)
    {
        Parent = parent;
    }

    public CultureInfo Parent { get; }
}
