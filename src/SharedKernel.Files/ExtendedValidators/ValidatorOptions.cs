using FluentValidation;
using FluentValidation.Internal;
using System;
using System.Linq.Expressions;

namespace SharedKernel.Files.ExtendedValidators
{
    public static class ValidatorOptions
    {
        public static IRuleBuilderOptions<T, TProperty> WhenIsNotNullOrWhitespace<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
            => rule.Configure(config => config.ApplyCondition(ctx => ctx.Instance != null && ctx.PropertyValue is string s && !string.IsNullOrWhiteSpace(s), applyConditionTo));

        public static IRuleBuilderOptions<T, TProperty> WithAdditionalMessageWhenPropertyIsValueNotNullOrEmptySpace<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<T, TProperty>> expression)
        {
            var name = expression.GetMember().Name;
            var propertyDisplayName = Extensions.SplitPascalCase(name);
            var additionalMessage = $"When '{propertyDisplayName}' is not null or empty space.";
            return rule.WithAdditionalMessage(additionalMessage);
        }

        public static IRuleBuilderOptions<T, TProperty> WithAdditionalMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string additionalMessage)
        {
            if (string.IsNullOrWhiteSpace(additionalMessage))
            {
                return rule;
            }

            return rule.Configure(config => config.MessageBuilder = context => context.GetDefaultMessage() + " " + additionalMessage);
        }
    }
}
