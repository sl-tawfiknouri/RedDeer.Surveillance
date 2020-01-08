namespace SharedKernel.Files.Allocations
{
    using FluentValidation;

    using SharedKernel.Files.Allocations.Interfaces;
    using SharedKernel.Files.ExtendedValidators;

    public class AllocationFileValidator : AbstractValidator<AllocationFileContract>, IAllocationFileValidator
    {
        public AllocationFileValidator()
        {
            this.RuleFor(x => x.OrderId).NotEmpty().MaximumLength(200);
            this.RuleFor(x => x.Fund).MaximumLength(200);
            this.RuleFor(x => x.Strategy).MaximumLength(200);
            this.RuleFor(x => x.ClientAccountId).MaximumLength(200);
            this.RuleFor(x => x.AllocationId).MaximumLength(200);
            this.RuleFor(x => x.OrderFilledVolume)
                .NotEmpty()
                .IsDecimal();
        }
    }
}