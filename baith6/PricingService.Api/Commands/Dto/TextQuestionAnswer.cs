namespace PricingService.Api.Commands.Dto;

public class TextQuestionAnswer : QuestionAnswer
{
    public string Answer { get; set; }

    public override object GetAnswer() => Answer;
}
