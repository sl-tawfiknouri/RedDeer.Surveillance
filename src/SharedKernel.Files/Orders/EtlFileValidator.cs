using FluentValidation;
using SharedKernel.Files.Orders.Interfaces;

namespace SharedKernel.Files.Orders
{
    public class EtlFileValidator : AbstractValidator<OrderFileContract>, IEtlFileValidator
    {

    }
}
