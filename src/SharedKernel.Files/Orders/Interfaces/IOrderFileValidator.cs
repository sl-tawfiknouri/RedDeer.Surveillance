namespace SharedKernel.Files.Orders.Interfaces
{
    using FluentValidation.Results;

    public interface IOrderFileValidator
    {
        ValidationResult Validate(OrderFileContract contract);
    }
}