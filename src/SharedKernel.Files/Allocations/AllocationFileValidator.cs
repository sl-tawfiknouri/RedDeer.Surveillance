namespace SharedKernel.Files.Allocations
{
    using FluentValidation;

    using SharedKernel.Files.Allocations.Interfaces;
    using SharedKernel.Files.PropertyValidators;

    public class AllocationFileValidator : AbstractValidator<AllocationFileContract>, IAllocationFileValidator
    {
        public AllocationFileValidator()
        {
            this.RuleFor(x => x.OrderId).NotEmpty().MaximumLength(255);
            this.RuleFor(x => x.Fund).MaximumLength(255);
            this.RuleFor(x => x.Strategy).MaximumLength(255);
            this.RuleFor(x => x.ClientAccountId).MaximumLength(255);
            this.RuleFor(x => x.OrderFilledVolume)
                .NotEmpty()
                .IsDecimal();
        }
    }
}