using FluentValidation;
using SharedKernel.Files.Allocations.Interfaces;
using SharedKernel.Files.PropertyValidators;

namespace SharedKernel.Files.Allocations
{
    public class AllocationFileValidator : AbstractValidator<AllocationFileContract>, IAllocationFileValidator
    {
        public AllocationFileValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Fund).MaximumLength(255);
            RuleFor(x => x.Strategy).MaximumLength(255);
            RuleFor(x => x.ClientAccountId).MaximumLength(255);
            RuleFor(x => x.OrderFilledVolume).NotEmpty().SetValidator(new DecimalParseableValidator("OrderFilledVolume"));
        }
    }
}
