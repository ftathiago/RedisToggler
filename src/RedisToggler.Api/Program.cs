using RedisToggler.Lib.Abstractions;
using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton(opt =>
    {
        return new CacheEntryConfiguration();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
