using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PricingService.Api.Commands.Dto;

namespace PricingService.Api.Converters;

internal sealed class QuestionAnswerConverter : JsonConverter<QuestionAnswer>
{
    public override QuestionAnswer ReadJson(
        JsonReader reader,
        Type objectType,
        QuestionAnswer existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        var json = JObject.Load(reader);
        var questionCode = json.Value<string>("questionCode")
            ?? throw new JsonSerializationException("Question code is required.");
        var typeToken = json["questionType"]
            ?? throw new JsonSerializationException("Question type is required.");
        var answerToken = json["answer"]
            ?? throw new JsonSerializationException("Answer is required.");

        var questionType = ParseQuestionType(typeToken);
        return questionType switch
        {
            QuestionType.Text => new TextQuestionAnswer
            {
                QuestionCode = questionCode,
                Answer = answerToken.Value<string>()
            },
            QuestionType.Numeric => new NumericQuestionAnswer
            {
                QuestionCode = questionCode,
                Answer = answerToken.Value<decimal>()
            },
            QuestionType.Choice => new ChoiceQuestionAnswer
            {
                QuestionCode = questionCode,
                Answer = answerToken.Value<string>()
            },
            _ => throw new JsonSerializationException($"Unsupported question type '{questionType}'.")
        };
    }

    public override void WriteJson(JsonWriter writer, QuestionAnswer value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("questionCode");
        serializer.Serialize(writer, value.QuestionCode);
        writer.WritePropertyName("questionType");
        serializer.Serialize(writer, value.QuestionType);
        writer.WritePropertyName("answer");
        serializer.Serialize(writer, value.GetAnswer());
        writer.WriteEndObject();
    }

    private static QuestionType ParseQuestionType(JToken token)
    {
        if (token.Type == JTokenType.Integer && Enum.IsDefined(typeof(QuestionType), token.Value<int>()))
        {
            return (QuestionType)token.Value<int>();
        }

        if (token.Type == JTokenType.String &&
            Enum.TryParse(token.Value<string>(), ignoreCase: true, out QuestionType questionType) &&
            Enum.IsDefined(questionType))
        {
            return questionType;
        }

        throw new JsonSerializationException($"Invalid question type '{token}'.");
    }
}
