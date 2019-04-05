using FluentValidation.Validators;

namespace SharedKernel.Files.PropertyValidators
{
    public class NonZeroLongParseableValidator : PropertyValidator
    {
        public NonZeroLongParseableValidator(string longPropertyName) : base($"Property had a value but could not be parsed to long {longPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop))
            {
                return true;
            }

            var parseResult = long.TryParse(prop, out var result);

            if (!parseResult)
            {
                return false;
            }

            return result != 0;
        }
    }
}
