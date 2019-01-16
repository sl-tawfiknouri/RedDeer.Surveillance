using FluentValidation.Results;

namespace DomainV2.Files.AllocationFile.Interfaces
{
    public interface IAllocationFileCsvValidator
    {
        ValidationResult Validate(AllocationFileCsv csv);
    }
}
