namespace PricingService.Api.Commands.Dto;

public class NumericQuestionAnswer : QuestionAnswer
{
    public decimal Answer { get; set; }

    public override object GetAnswer() => Answer;
}
