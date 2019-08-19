namespace SharedKernel.Files.Allocations.Interfaces
{
    using FluentValidation.Results;

    public interface IAllocationFileValidator
    {
        ValidationResult Validate(AllocationFileContract contract);
    }
}