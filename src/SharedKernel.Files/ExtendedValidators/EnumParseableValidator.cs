using System;
using Domain.Core.Extensions;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace SharedKernel.Files.ExtendedValidators
{
    
    public class EnumParseableValidator<T> : PropertyValidator
        where T : struct, IConvertible
    {
        public EnumParseableValidator()
            : base("'{PropertyName}' value '{PropertyValue}' must be assignable to typeof<{EnumType}> enum.")
        {
        }

        protected override ValidationFailure CreateValidationError(PropertyValidatorContext context)
        {
            context.MessageFormatter.AppendArgument("EnumType", typeof(T));
            return base.CreateValidationError(context);
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;
            var parseable = EnumExtensions.TryParsePermutations(prop, out T result);
            return parseable;
        }
    }
}