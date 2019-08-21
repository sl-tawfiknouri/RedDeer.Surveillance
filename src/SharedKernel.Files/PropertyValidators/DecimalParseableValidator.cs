namespace SharedKernel.Files.PropertyValidators
{
    using FluentValidation.Validators;

    public class DecimalParseableValidator : PropertyValidator
    {
        public DecimalParseableValidator(string decimalPropertyName)
            : base($"Property had a value but could not be parsed to decimal {decimalPropertyName}")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop)) return true;

            return decimal.TryParse(prop, out var result);
        }
    }
}