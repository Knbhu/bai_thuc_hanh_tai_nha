using Newtonsoft.Json;
using PricingService.Api.Converters;

namespace PricingService.Api.Commands.Dto;

[JsonConverter(typeof(QuestionAnswerConverter))]
public abstract class QuestionAnswer
{
    public string QuestionCode { get; set; }

    public abstract QuestionType QuestionType { get; }

    public abstract object GetAnswer();
}

public enum QuestionType
{
    Text,
    Numeric,
    Choice
}
