using FluentValidation.Results;

namespace Domain.Files.AllocationFile.Interfaces
{
    public interface IAllocationFileCsvValidator
    {
        ValidationResult Validate(AllocationFileCsv csv);
    }
}
