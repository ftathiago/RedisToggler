using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using RedisToggler.Api;
using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Extensions;
using System.Globalization;

CultureInfo _allCulturesCulture = new("all");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddLocalization()
    .Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new CultureInfo[]
        {
            new CultureInfo("pt-BR"),
            new CultureInfo("en"),
            new CultureInfo("es"),
        };
        options.SetDefaultCulture("pt-BR");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.ApplyCurrentCultureToResponseHeaders = true;
        options.RequestCultureProviders.Clear();
        options.RequestCultureProviders.Add(new SupportedRequestCultureProvider());
    })
    .AddScoped(provider =>
    {
        var contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
        if (contextAccessor.HttpContext is null)
        {
            return SupportedCultures.Default;
        }

        var cultureFeature = contextAccessor.HttpContext!.Features.Get<IRequestCultureFeature>();
        var requestCulture = cultureFeature!.RequestCulture.Culture;
        if (requestCulture.Equals(_allCulturesCulture))
        {
            var localizationOptions = provider.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            var supportedCultures = localizationOptions.SupportedCultures!.ToArray();

            return new SupportedCultures(
                parent: requestCulture,
                list: supportedCultures[..^1]);
        }

        return new SupportedCultures(requestCulture);
    })
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options => options.OperationFilter<ApplicationHeaders>())
    .AddSingleton(opt => new CacheEntryConfiguration()
    {
        StoreLanguage = true,
    })
    .AddCacheWrapper(opt =>
    {
        opt.ConnectionString = "localhost:6379,asyncTimeout=1000,connectTimeout=1000,password=eYVX7EwVmmxKPCDmwMtyKVge8oLd2t81,abortConnect=false";
        opt.CacheType = CacheType.Redis;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>()!.Value);
app.UseAuthorization();

app.MapControllers();

app.Run();
