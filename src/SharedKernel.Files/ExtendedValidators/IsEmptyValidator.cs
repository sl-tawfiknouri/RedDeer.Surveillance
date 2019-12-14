using FluentValidation.Resources;
using FluentValidation.Validators;

namespace SharedKernel.Files.ExtendedValidators
{
    public class IsEmptyValidator
        : EmptyValidator
    {
        public IsEmptyValidator(object defaultValueForType)
            : base(defaultValueForType)
        {
            Options.ErrorMessageSource = new StaticStringSource("'{PropertyName}' with value '{PropertyValue}' must be empty.");
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue is string s)
            {
                return string.IsNullOrEmpty(s);
            }

            return base.IsValid(context);
        }
    }
}
