using Domain.Files.AllocationFile.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace Domain.Files.AllocationFile
{
    public class AllocationFileCsvValidator : AbstractValidator<AllocationFileCsv>, IAllocationFileCsvValidator
    {
        public AllocationFileCsvValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Fund).MaximumLength(255);
            RuleFor(x => x.Strategy).MaximumLength(255);
            RuleFor(x => x.ClientAccountId).MaximumLength(255);
            RuleFor(x => x.OrderFilledVolume).NotEmpty().SetValidator(new LongParseableValidator("OrderFilledVolume"));
        }

        public class LongParseableValidator : PropertyValidator
        {
            public LongParseableValidator(string longPropertyName) : base($"Property had a value but could not be parsed to long {longPropertyName}")
            { }

            protected override bool IsValid(PropertyValidatorContext context)
            {
                var prop = context.PropertyValue as string;

                if (string.IsNullOrWhiteSpace(prop))
                {
                    return true;
                }

                return long.TryParse(prop, out var result);
            }
        }
    }
}
