namespace PricingService.Api.Commands.Dto;

public abstract class QuestionAnswer
{
    public string QuestionCode { get; set; }

    public abstract object GetAnswer();
}
