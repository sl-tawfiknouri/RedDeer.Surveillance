namespace SharedKernel.Files.PropertyValidators
{
    using System;
    using System.Globalization;

    using FluentValidation.Validators;

    public class DateParseableValidator : PropertyValidator
    {
        public DateParseableValidator(string errorMessage)
            : base(errorMessage)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop)) return true;

            var hasDateOnlyFormat = DateTime.TryParseExact(
                prop,
                "yyyy-MM-dd",
                null,
                DateTimeStyles.None,
                out var dateOnlyResult);

            if (hasDateOnlyFormat && dateOnlyResult.Year >= 2010) return true;

            if (hasDateOnlyFormat && dateOnlyResult.Year < 2010)
            {
                context.MessageFormatter.AppendArgument(
                    context.PropertyName,
                    $"{prop} has a year preceding 2010 which is out of range");
                return false;
            }

            var hasDateAndTimeFormat = DateTime.TryParseExact(
                prop,
                "yyyy-MM-ddTHH:mm:ss",
                null,
                DateTimeStyles.None,
                out var dateAndTimeResult);

            if (hasDateAndTimeFormat && dateAndTimeResult.Year >= 2010) return true;

            if (hasDateAndTimeFormat && dateAndTimeResult.Year < 2010)
            {
                context.MessageFormatter.AppendArgument(
                    context.PropertyName,
                    $"{prop} has a year preceding 2010 which is out of range");
                return false;
            }

            context.MessageFormatter.AppendArgument(
                context.PropertyName,
                $"{prop} could not recognise date provided as being correctly formatted");
            return false;
        }
    }
}