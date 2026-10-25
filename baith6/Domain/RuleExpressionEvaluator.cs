using System;
using System.Collections.Generic;
using System.Globalization;

namespace PricingService.Domain;

internal static class RuleExpressionEvaluator
{
    public static bool EvaluateBoolean(string expression, IDictionary<string, object> values)
    {
        var value = new Parser(expression, values).Parse();
        return value is bool result
            ? result
            : throw new InvalidOperationException($"Rule expression '{expression}' did not return a boolean.");
    }

    public static decimal EvaluateDecimal(string expression, IDictionary<string, object> values)
    {
        var value = new Parser(expression, values).Parse();
        return value is decimal result
            ? result
            : throw new InvalidOperationException($"Price expression '{expression}' did not return a number.");
    }

    private sealed class Parser
    {
        private readonly string expression;
        private readonly IDictionary<string, object> values;
        private int position;

        public Parser(string expression, IDictionary<string, object> values)
        {
            this.expression = expression;
            this.values = values;
        }

        public object Parse()
        {
            var result = ParseConditional();
            SkipWhitespace();
            if (position != expression.Length)
            {
                throw Error("Unexpected trailing input.");
            }

            return result;
        }

        private object ParseConditional()
        {
            var condition = ParseComparison();
            if (!Match("?"))
            {
                return condition;
            }

            var whenTrue = ParseConditional();
            Expect(":");
            var whenFalse = ParseConditional();
            return ToBoolean(condition) ? whenTrue : whenFalse;
        }

        private object ParseComparison()
        {
            var left = ParseAdditive();
            foreach (var operation in new[] { "==", "!=", ">=", "<=", ">", "<" })
            {
                if (!Match(operation))
                {
                    continue;
                }

                var right = ParseAdditive();
                var comparison = Compare(left, right);
                return operation switch
                {
                    "==" => comparison == 0,
                    "!=" => comparison != 0,
                    ">" => comparison > 0,
                    "<" => comparison < 0,
                    ">=" => comparison >= 0,
                    "<=" => comparison <= 0,
                    _ => throw Error($"Unsupported comparison operator '{operation}'.")
                };
            }

            return left;
        }

        private object ParseAdditive()
        {
            var value = ParseMultiplicative();
            while (true)
            {
                if (Match("+"))
                {
                    value = ToDecimal(value) + ToDecimal(ParseMultiplicative());
                }
                else if (Match("-"))
                {
                    value = ToDecimal(value) - ToDecimal(ParseMultiplicative());
                }
                else
                {
                    return value;
                }
            }
        }

        private object ParseMultiplicative()
        {
            var value = ParseUnary();
            while (true)
            {
                if (Match("*"))
                {
                    value = ToDecimal(value) * ToDecimal(ParseUnary());
                }
                else if (Match("/"))
                {
                    value = ToDecimal(value) / ToDecimal(ParseUnary());
                }
                else
                {
                    return value;
                }
            }
        }

        private object ParseUnary()
        {
            if (Match("-"))
            {
                return -ToDecimal(ParseUnary());
            }

            if (Match("+"))
            {
                return ToDecimal(ParseUnary());
            }

            return ParsePrimary();
        }

        private object ParsePrimary()
        {
            SkipWhitespace();
            if (Match("("))
            {
                var nestedValue = ParseConditional();
                Expect(")");
                return nestedValue;
            }

            if (position < expression.Length && expression[position] == '"')
            {
                return ParseString();
            }

            if (position < expression.Length && (char.IsDigit(expression[position]) || expression[position] == '.'))
            {
                return ParseNumber();
            }

            var identifier = ParseIdentifier();
            if (!values.TryGetValue(identifier, out var value))
            {
                throw Error($"No value was supplied for '{identifier}'.");
            }

            return value switch
            {
                decimal number => number,
                int number => (decimal)number,
                long number => (decimal)number,
                _ => value
            };
        }

        private string ParseString()
        {
            position++;
            var start = position;
            while (position < expression.Length && expression[position] != '"')
            {
                if (expression[position] == '\\' && position + 1 < expression.Length)
                {
                    position++;
                }

                position++;
            }

            if (position >= expression.Length)
            {
                throw Error("Unterminated string literal.");
            }

            var value = expression[start..position].Replace("\\\"", "\"").Replace("\\\\", "\\");
            position++;
            return value;
        }

        private decimal ParseNumber()
        {
            SkipWhitespace();
            var start = position;
            while (position < expression.Length &&
                   (char.IsDigit(expression[position]) || expression[position] == '.'))
            {
                position++;
            }

            var value = decimal.Parse(expression[start..position], CultureInfo.InvariantCulture);
            if (position < expression.Length && (expression[position] == 'M' || expression[position] == 'm'))
            {
                position++;
            }

            return value;
        }

        private string ParseIdentifier()
        {
            SkipWhitespace();
            var start = position;
            while (position < expression.Length &&
                   (char.IsLetterOrDigit(expression[position]) || expression[position] == '_'))
            {
                position++;
            }

            if (start == position)
            {
                throw Error("Expected a value.");
            }

            return expression[start..position];
        }

        private bool Match(string token)
        {
            SkipWhitespace();
            if (!expression.AsSpan(position).StartsWith(token, StringComparison.Ordinal))
            {
                return false;
            }

            position += token.Length;
            return true;
        }

        private void Expect(string token)
        {
            if (!Match(token))
            {
                throw Error($"Expected '{token}'.");
            }
        }

        private void SkipWhitespace()
        {
            while (position < expression.Length && char.IsWhiteSpace(expression[position]))
            {
                position++;
            }
        }

        private int Compare(object left, object right)
        {
            if (left is string leftText && right is string rightText)
            {
                return string.CompareOrdinal(leftText, rightText);
            }

            return ToDecimal(left).CompareTo(ToDecimal(right));
        }

        private static decimal ToDecimal(object value)
        {
            return value switch
            {
                decimal number => number,
                int number => number,
                long number => number,
                _ => decimal.Parse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
            };
        }

        private static bool ToBoolean(object value)
        {
            return value is bool result
                ? result
                : throw new InvalidOperationException("Conditional expressions require a boolean condition.");
        }

        private FormatException Error(string message)
        {
            return new FormatException($"{message} At position {position} in '{expression}'.");
        }
    }
}
