using System;
using System.Collections.Generic;
using DashboardService.Domain;
using DashboardService.Messaging.RabbitMq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PolicyService.Api.Events;
using Steeltoe.Discovery.Eureka;
using DashboardService.DataAccess.Sql;

namespace DashboardService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        if (Configuration.GetValue<bool?>("LocalDevelopment:UseServiceDiscovery") != false)
            services.AddEurekaDiscoveryClient();
        services.AddMvc()
            .AddNewtonsoftJson();
        services.AddMediatR(opts => opts.RegisterServicesFromAssemblyContaining<Startup>());
        services.AddSingleton<IPolicyRepository>(_ =>
            new SqlPolicyRepository(Configuration.GetConnectionString("SqlServer")));
        if (Configuration.GetValue<bool?>("RabbitMqOptions:Enabled") != false)
            services.AddRabbitListeners(Configuration.GetSection("RabbitMqOptions").Get<RabbitMqOptions>());
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
        
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        if (Configuration.GetValue<bool?>("RabbitMqOptions:Enabled") != false)
            app.UseRabbitListeners(new List<Type> { typeof(PolicyCreated) });
    }
}