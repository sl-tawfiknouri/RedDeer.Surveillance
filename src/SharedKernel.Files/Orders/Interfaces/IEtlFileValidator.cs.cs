namespace SharedKernel.Files.Orders.Interfaces
{
    using FluentValidation.Results;

    public interface IEtlFileValidator
    {
        ValidationResult Validate(OrderFileContract contract);
    }
}