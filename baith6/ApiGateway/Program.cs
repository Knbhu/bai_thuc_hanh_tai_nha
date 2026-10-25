using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:8099");
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: false);
builder.Services
    .AddOcelot()
    .AddEureka()
    .AddCacheManager(options => options.WithDictionaryHandle());

var app = builder.Build();

await app.UseOcelot();
await app.RunAsync();
