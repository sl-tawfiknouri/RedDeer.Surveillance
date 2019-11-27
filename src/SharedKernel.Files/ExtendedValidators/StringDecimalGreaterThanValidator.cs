using FluentValidation.Resources;
using FluentValidation.Validators;
using System;

namespace SharedKernel.Files.PropertyValidators
{
    public class StringDecimalGreaterThanValidator
        : GreaterThanValidator
    {
        public StringDecimalGreaterThanValidator(long value)
            : base(value)
        {
            Options.ErrorMessageSource = new StaticStringSource("'{PropertyName}' with value '{PropertyValue}' must be greater than '{ComparisonValue}'.");
        }

        public override bool IsValid(IComparable value, IComparable valueToCompare)
        {
            if(!decimal.TryParse(value?.ToString(), out decimal result))
            {
                return false;
            }

            return base.IsValid(result, valueToCompare);
        }
    }
}
