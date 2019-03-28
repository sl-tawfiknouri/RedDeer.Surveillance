using FluentValidation.Results;

namespace SharedKernel.Files.Orders.Interfaces
{
    public interface IEtlFileValidator
    {
        ValidationResult Validate(OrderFileContract contract);
    }
}
