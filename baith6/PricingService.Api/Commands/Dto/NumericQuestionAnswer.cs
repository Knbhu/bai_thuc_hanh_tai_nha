namespace PricingService.Api.Commands.Dto;

public class NumericQuestionAnswer : QuestionAnswer
{
    public decimal Answer { get; set; }

    public override QuestionType QuestionType => QuestionType.Numeric;

    public override object GetAnswer() => Answer;
}
