using System;
using System.Globalization;
using FluentValidation.Validators;

namespace SharedKernel.Files.PropertyValidators
{
    public class EnumParseableValidator<T> : PropertyValidator where T : struct, IConvertible
    {
        public EnumParseableValidator(string enumPropertyName) : base($"Property out of enum range {enumPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (prop == null)
            {
                context.MessageFormatter.AppendArgument(context.PropertyName, typeof(T));

                return false;
            }

            var textInfo = new CultureInfo("en-GB", false).TextInfo;

            if (Enum.TryParse(prop, out T result1))
            {
                return true;
            }
            else if ((Enum.TryParse(prop.ToLower(), out T result2)))
            {
                return true;
            }
            else if ((Enum.TryParse(prop.ToUpper(), out T result3)))
            {
                return true;
            }
            else if ((Enum.TryParse(textInfo.ToTitleCase(prop.ToLower()), out T result4)))
            {
                return true;
            }

            context.MessageFormatter.AppendArgument(context.PropertyName, typeof(T));

            return false;
        }
    }
}
