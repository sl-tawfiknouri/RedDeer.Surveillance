using FluentValidation.Results;

namespace SharedKernel.Files.Allocations.Interfaces
{
    public interface IAllocationFileValidator
    {
        ValidationResult Validate(AllocationFileContract contract);
    }
}
