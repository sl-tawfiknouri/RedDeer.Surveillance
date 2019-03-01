using FluentValidation.Results;

namespace SharedKernel.Files.Allocations.Interfaces
{
    public interface IAllocationFileCsvValidator
    {
        ValidationResult Validate(AllocationFileContract contract);
    }
}
