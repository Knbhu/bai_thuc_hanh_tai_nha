using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using PricingService.Api.Commands;
using RestEase;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace PolicyService.RestClients;

public interface IPricingClient
{
    [Post]
    Task<CalculatePriceResult> CalculatePrice([Body] CalculatePriceCommand cmd);
}

public class PricingClient : IPricingClient
{
    private static readonly AsyncRetryPolicy retryPolicy = Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3));

    private readonly IPricingClient client;

    public PricingClient(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        var discoveryClient = serviceProvider.GetService<IDiscoveryClient>();
        var useServiceDiscovery = configuration.GetValue<bool?>("LocalDevelopment:UseServiceDiscovery") != false;
        var handler = useServiceDiscovery
            ? new DiscoveryHttpClientHandler(discoveryClient ??
                throw new InvalidOperationException("Service discovery is enabled but no discovery client is registered."))
            : new HttpClientHandler();
        var httpClient = new HttpClient(handler, false)
        {
            BaseAddress = new Uri(configuration.GetValue<string>("PricingServiceUri"))
        };
        client = RestClient.For<IPricingClient>(httpClient);
    }

    public Task<CalculatePriceResult> CalculatePrice([Body] CalculatePriceCommand cmd)
    {
        return retryPolicy.ExecuteAsync(async () => await client.CalculatePrice(cmd));
    }
}