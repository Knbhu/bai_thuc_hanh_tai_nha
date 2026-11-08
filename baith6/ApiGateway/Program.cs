using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:8099");
var ocelotConfig = builder.Environment.IsDevelopment() ? "ocelot.Development.json" : "ocelot.json";
builder.Configuration.AddJsonFile(ocelotConfig, optional: false, reloadOnChange: false);

var ocelot = builder.Services.AddOcelot();
if (!builder.Environment.IsDevelopment())
{
    ocelot.AddEureka();
}

ocelot.AddCacheManager(options => options.WithDictionaryHandle());

var app = builder.Build();

await app.UseOcelot();
await app.RunAsync();
