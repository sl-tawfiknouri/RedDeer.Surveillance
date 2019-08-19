namespace SharedKernel.Files.PropertyValidators
{
    using FluentValidation.Validators;

    public class LongParseableValidator : PropertyValidator
    {
        public LongParseableValidator(string longPropertyName)
            : base($"Property had a value but could not be parsed to long {longPropertyName}")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop)) return true;

            return long.TryParse(prop, out var result);
        }
    }
}