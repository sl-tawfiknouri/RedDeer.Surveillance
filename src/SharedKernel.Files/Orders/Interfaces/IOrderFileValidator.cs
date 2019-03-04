using FluentValidation.Results;

namespace SharedKernel.Files.Orders.Interfaces
{
    public interface IOrderFileValidator
    {
        ValidationResult Validate(OrderFileContract contract);
    }
}
