using System;
using System.Globalization;
using System.Linq;
using FluentValidation.Validators;

namespace SharedKernel.Files.ExtendedValidators
{
    public class DateParseableValidator 
        : PropertyValidator
    {
        public DateParseableValidator()
            : base("'{PropertyName}' must have a valid date. {AdditionalErrorMessage}")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var propertyValue = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(propertyValue)) 
                return true;

            var dateFormats = new string[] { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd" };

            foreach (var dateFormat in dateFormats)
            {
                if (!DateTime.TryParseExact(propertyValue, dateFormat, null, DateTimeStyles.None, out var dateOnlyResult))
                {
                    continue;
                }

                if (dateOnlyResult.Year >= 2010)
                {
                    return true;
                }

                context.MessageFormatter.AppendArgument("AdditionalErrorMessage", "'{PropertyValue}' has a year preceding 2010 which is out of range.");
                return false;
            }

            context.MessageFormatter.AppendArgument("AdditionalErrorMessage", "'{PropertyValue}' is not recognised using formats: [" + string.Join(", ", dateFormats.Select(s => $"'{s}'") ) + "]."); 
            return false;
        }
    }
}