using FluentValidation;
using System;

namespace SharedKernel.Files.ExtendedValidators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> IsEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            => ruleBuilder
                .SetValidator(new IsEmptyValidator(default(TProperty)));

        public static IRuleBuilderOptions<T, TProperty> IsNotEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            => ruleBuilder
                .SetValidator(new IsNotEmptyValidator(default(TProperty)));

        public static IRuleBuilderOptions<T, TProperty> IsParseableDate<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            => ruleBuilder
                .SetValidator(new DateParseableValidator());

        public static IRuleBuilderOptions<T, TProperty> IsDecimal<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            => ruleBuilder
                .SetValidator(new IsDecimalStringValidator());
        

        public static IRuleBuilderOptions<T, TProperty> IsDecimalWhenIsNotNullOrWhitespace<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            => ruleBuilder
                .IsDecimal()
                .WhenIsNotNullOrWhitespace();
        
        public static IRuleBuilderOptions<T, TProperty> StringDecimalGreaterThan<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            long valueToCompare)
            where TProperty : IComparable<TProperty>, IComparable
        {
            return ruleBuilder
                .SetValidator(new StringDecimalGreaterThanValidator(valueToCompare));
        }
    }
}
