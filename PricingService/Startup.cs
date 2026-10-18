using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PricingService.Commands;
using PricingService.Domain;
using Steeltoe.Discovery.Eureka;

namespace PricingService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEurekaDiscoveryClient();
        services.AddControllers().AddNewtonsoftJson();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(CalculatePriceHandler).Assembly));
        services.AddSingleton<IDataStore, InMemoryDataStore>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
