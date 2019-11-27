using FluentValidation.Resources;
using FluentValidation.Validators;

namespace SharedKernel.Files.PropertyValidators
{
    public class IsNotEmptyValidator
        : NotEmptyValidator
    {
        public IsNotEmptyValidator(object defaultValueForType)
            : base(defaultValueForType)
        {
            Options.ErrorMessageSource = new StaticStringSource("'{PropertyName}' with value '{PropertyValue}' must not be empty.");
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue is string s && string.IsNullOrEmpty(s))
            {
                return false;
            }

            return base.IsValid(context);
        }
    }
}
