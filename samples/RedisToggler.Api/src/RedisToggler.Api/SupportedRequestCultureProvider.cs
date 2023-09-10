using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;

namespace RedisToggler.Api;

public sealed class SupportedRequestCultureProvider : AcceptLanguageHeaderRequestCultureProvider
{
    private const string AllCulturesSymbol = "*";

    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var acceptLanguageHeader = httpContext.Request.Headers[HeaderNames.AcceptLanguage];
        if (acceptLanguageHeader.SequenceEqual(new[] { AllCulturesSymbol }))
        {
            var result = new ProviderCultureResult("all");
            return Task.FromResult<ProviderCultureResult?>(result);
        }

        return base.DetermineProviderCultureResult(httpContext);
    }
}
