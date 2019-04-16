using FluentValidation.Validators;

namespace SharedKernel.Files.PropertyValidators
{
    public class NonZeroDecimalParseableValidator : PropertyValidator
    {
        public NonZeroDecimalParseableValidator(string decimalPropertyName) : base($"Property had a value but could not be parsed to a non decimal {decimalPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop))
            {
                return true;
            }

            var parseResult = decimal.TryParse(prop, out var result);

            if (!parseResult)
            {
                return false;
            }

            return result != 0;
        }
    }
}
