using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PricingService.Api.Commands;
using PricingService.Domain;

namespace PricingService.Commands;

public class CalculatePriceHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResult>
{
    private readonly IDataStore dataStore;
    private readonly CalculatePriceCommandValidator validator = new();

    public CalculatePriceHandler(IDataStore dataStore)
    {
        this.dataStore = dataStore;
    }

    public async Task<CalculatePriceResult> Handle(CalculatePriceCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var tariff = await dataStore.Tariffs[request.ProductCode];
        var subject = request.Answers.ToDictionary(answer => answer.QuestionCode, answer => answer.GetAnswer());
        var calculation = new Calculation(
            request.ProductCode,
            request.PolicyFrom,
            request.PolicyTo,
            request.SelectedCovers,
            subject);
        var price = tariff.CalculatePrice(calculation);

        return new CalculatePriceResult
        {
            TotalPrice = price.TotalPremium,
            CoverPrices = price.Covers.ToDictionary(cover => cover.Key, cover => cover.Value.Price)
        };
    }
}
