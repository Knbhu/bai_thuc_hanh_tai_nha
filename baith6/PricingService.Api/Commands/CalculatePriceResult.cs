using System.Collections.Generic;

namespace PricingService.Api.Commands;

public class CalculatePriceResult
{
    public decimal TotalPrice { get; set; }
    public IDictionary<string, decimal> CoverPrices { get; set; }
}
