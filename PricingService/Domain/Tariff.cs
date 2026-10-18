using System;
using System.Collections.Generic;
using System.Linq;

namespace PricingService.Domain;

public class Tariff
{
    public Tariff(string code)
    {
        Code = code;
        BasePremiumRules = new BasePremiumRules();
        DiscountMarkupRules = new DiscountMarkupRules();
    }

    public string Code { get; }
    public BasePremiumRules BasePremiumRules { get; }
    public DiscountMarkupRules DiscountMarkupRules { get; }

    public TariffPrice CalculatePrice(Calculation calculation)
    {
        var covers = new Dictionary<string, CoverPrice>();
        foreach (var coverCode in calculation.SelectedCovers)
        {
            var rule = BasePremiumRules.Rules.FirstOrDefault(candidate =>
                candidate.CoverCode == coverCode &&
                (candidate.Condition == null || RuleExpressionEvaluator.EvaluateBoolean(candidate.Condition, calculation.Subject)));

            if (rule == null)
            {
                throw new InvalidOperationException($"No pricing rule applies to cover '{coverCode}' for tariff '{Code}'.");
            }

            var amount = RuleExpressionEvaluator.EvaluateDecimal(rule.Expression, calculation.Subject);
            foreach (var markup in DiscountMarkupRules.Rules)
            {
                if (RuleExpressionEvaluator.EvaluateBoolean(markup.Condition, calculation.Subject))
                {
                    amount *= markup.Multiplier;
                }
            }

            covers.Add(coverCode, new CoverPrice(amount));
        }

        return new TariffPrice(covers);
    }
}

public class TariffPrice
{
    public TariffPrice(Dictionary<string, CoverPrice> covers)
    {
        Covers = covers;
        TotalPremium = covers.Values.Sum(cover => cover.Price);
    }

    public decimal TotalPremium { get; }
    public Dictionary<string, CoverPrice> Covers { get; }
}

public class CoverPrice
{
    public CoverPrice(decimal price)
    {
        Price = price;
    }

    public decimal Price { get; }
}

public class BasePremiumRules
{
    public List<BasePriceRule> Rules { get; } = new();

    public void AddBasePriceRule(string coverCode, string condition, string expression)
    {
        Rules.Add(new BasePriceRule(coverCode, condition, expression));
    }
}

public class BasePriceRule
{
    public BasePriceRule(string coverCode, string condition, string expression)
    {
        CoverCode = coverCode;
        Condition = condition;
        Expression = expression;
    }

    public string CoverCode { get; }
    public string Condition { get; }
    public string Expression { get; }
}

public class DiscountMarkupRules
{
    public List<DiscountMarkupRule> Rules { get; } = new();

    public void AddPercentMarkup(string condition, decimal multiplier)
    {
        Rules.Add(new DiscountMarkupRule(condition, multiplier));
    }
}

public class DiscountMarkupRule
{
    public DiscountMarkupRule(string condition, decimal multiplier)
    {
        Condition = condition;
        Multiplier = multiplier;
    }

    public string Condition { get; }
    public decimal Multiplier { get; }
}
