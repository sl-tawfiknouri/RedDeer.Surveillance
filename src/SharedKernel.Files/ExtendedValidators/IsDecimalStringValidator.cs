using FluentValidation.Validators;

namespace SharedKernel.Files.ExtendedValidators
{
    public class IsDecimalStringValidator 
        : PropertyValidator
    {
        public IsDecimalStringValidator()
            : base("'{PropertyName}' value '{PropertyValue}' must be a valid decimal type.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var propertyValue = context.PropertyValue as string;
            return decimal.TryParse(propertyValue, out var result);
        }
    }
}