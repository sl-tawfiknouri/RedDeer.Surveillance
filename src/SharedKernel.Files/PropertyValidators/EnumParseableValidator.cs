using System;
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

            if (!Enum.TryParse(prop, out T result))
            {
                context.MessageFormatter.AppendArgument(context.PropertyName, typeof(T));

                return false;
            }

            return true;
        }
    }
}
