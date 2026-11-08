namespace PricingService.Api.Commands.Dto;

public class ChoiceQuestionAnswer : QuestionAnswer
{
    public string Answer { get; set; }

    public override QuestionType QuestionType => QuestionType.Choice;

    public override object GetAnswer() => Answer;
}
