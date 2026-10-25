using FluentValidation;

namespace PricingService.Api.Commands;

public class CalculatePriceCommandValidator : AbstractValidator<CalculatePriceCommand>
{
    public CalculatePriceCommandValidator()
    {
        RuleFor(command => command.ProductCode).NotEmpty();
        RuleFor(command => command.SelectedCovers).NotEmpty();
        RuleFor(command => command.Answers).NotEmpty();
    }
}
