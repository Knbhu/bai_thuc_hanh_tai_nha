using System.Collections.Generic;
using System.Threading.Tasks;

namespace PricingService.Domain;

public class InMemoryDataStore : Commands.IDataStore
{
    public InMemoryDataStore()
    {
        Tariffs = new Dictionary<string, Task<Tariff>>
        {
            ["TRI"] = Task.FromResult(CreateTravelTariff()),
            ["HSI"] = Task.FromResult(CreateHouseTariff()),
            ["FAI"] = Task.FromResult(CreateFarmTariff()),
            ["CAR"] = Task.FromResult(CreateCarTariff())
        };
    }

    public IDictionary<string, Task<Tariff>> Tariffs { get; }

    private static Tariff CreateTravelTariff()
    {
        var tariff = new Tariff("TRI");
        tariff.BasePremiumRules.AddBasePriceRule("C1", null,
            "(NUM_OF_ADULTS) * (DESTINATION == \"EUR\" ? 26.00M : 34.00M)");
        tariff.BasePremiumRules.AddBasePriceRule("C2", null, "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 26.00M");
        tariff.BasePremiumRules.AddBasePriceRule("C3", null, "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 10.00M");
        tariff.DiscountMarkupRules.AddPercentMarkup("DESTINATION == \"WORLD\"", 1.5M);
        return tariff;
    }

    private static Tariff CreateHouseTariff()
    {
        var tariff = new Tariff("HSI");
        tariff.BasePremiumRules.AddBasePriceRule("C1", "TYP == \"APT\"", "AREA * 1.25M");
        tariff.BasePremiumRules.AddBasePriceRule("C1", "TYP == \"HOUSE\"", "AREA * 1.50M");
        tariff.BasePremiumRules.AddBasePriceRule("C2", "TYP == \"APT\"", "AREA * 0.25M");
        tariff.BasePremiumRules.AddBasePriceRule("C2", "TYP == \"HOUSE\"", "AREA * 0.45M");
        tariff.BasePremiumRules.AddBasePriceRule("C3", null, "30M");
        tariff.BasePremiumRules.AddBasePriceRule("C4", null, "50M");
        tariff.DiscountMarkupRules.AddPercentMarkup("FLOOD == \"YES\"", 1.50M);
        tariff.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 1", 1.25M);
        return tariff;
    }

    private static Tariff CreateFarmTariff()
    {
        var tariff = new Tariff("FAI");
        tariff.BasePremiumRules.AddBasePriceRule("C1", null, "10M");
        tariff.BasePremiumRules.AddBasePriceRule("C2", null, "20M");
        tariff.BasePremiumRules.AddBasePriceRule("C3", null, "30M");
        tariff.BasePremiumRules.AddBasePriceRule("C4", null, "40M");
        tariff.DiscountMarkupRules.AddPercentMarkup("FLOOD == \"YES\"", 1.50M);
        tariff.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 2", 2.00M);
        return tariff;
    }

    private static Tariff CreateCarTariff()
    {
        var tariff = new Tariff("CAR");
        tariff.BasePremiumRules.AddBasePriceRule("C1", null, "100M");
        tariff.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 2", 1.50M);
        return tariff;
    }
}
